using DaybreakGames.Census.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Microservice.Cache;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.Microservice.Utility;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class CharacterDirectiveStore : ICharacterDirectiveStore
    {
        private readonly ICharacterRepository _repository;
        private readonly CharactersDirectiveCollection _charactersDirectiveCollection;
        private readonly CharactersDirectiveObjectiveCollection _charactersDirectiveObjectiveCollection;
        private readonly CharactersDirectiveTierCollection _charactersDirectiveTierCollection;
        private readonly CharactersDirectiveTreeCollection _charactersDirectiveTreeCollection;
        private readonly IStoreUpdaterHelper _storeUpdaterHelper;
        private readonly ICache _cache;

        private const string _cacheKey = "ps2.characterDirectiveStore";
        private readonly Func<string, string> _getCharacterDirectiveCacheKey = characterId => $"{_cacheKey}_directives_{characterId}";

        private readonly KeyedSemaphoreSlim _characterLock = new KeyedSemaphoreSlim();

        public CharacterDirectiveStore(
            ICharacterRepository repository,
            CharactersDirectiveCollection charactersDirectiveCollection,
            CharactersDirectiveObjectiveCollection charactersDirectiveObjectiveCollection,
            CharactersDirectiveTierCollection charactersDirectiveTierCollection,
            CharactersDirectiveTreeCollection charactersDirectiveTreeCollection,
            IStoreUpdaterHelper storeUpdaterHelper,
            ICache cache)
        {
            _repository = repository;
            _charactersDirectiveCollection = charactersDirectiveCollection;
            _charactersDirectiveObjectiveCollection = charactersDirectiveObjectiveCollection;
            _charactersDirectiveTierCollection = charactersDirectiveTierCollection;
            _charactersDirectiveTreeCollection = charactersDirectiveTreeCollection;
            _storeUpdaterHelper = storeUpdaterHelper;
            _cache = cache;
        }

        public async Task<IEnumerable<CharacterDirectiveTree>> GetCharacterDirectivesAsync(string characterId)
        {
            using (await _characterLock.WaitAsync(characterId))
            {
                var data = await _repository.GetCharacterDirectivesAsync(characterId);
                if (data == null || !data.Any())
                {
                    try
                    {
                        await UpdateCharacterDirectiveDataAsync(characterId);
                        data = await _repository.GetCharacterDirectivesAsync(characterId);
                    }
                    catch (CensusConnectionException)
                    {
                        return null;
                    }
                }

                return data;
            }
        }

        public async Task UpdateCharacterDirectiveDataAsync(string characterId)
        {
            await Task.WhenAll(
                _storeUpdaterHelper.UpdateAsync<CensusCharacterDirectiveModel, CharacterDirective>(
                    () => _charactersDirectiveCollection.GetCharacterDirectivesAsync(characterId)),

                _storeUpdaterHelper.UpdateAsync<CensusCharacterDirectiveTierModel, CharacterDirectiveTier>(
                    () => _charactersDirectiveTierCollection.GetCharacterDirectiveTiersAsync(characterId)),

                _storeUpdaterHelper.UpdateAsync<CensusCharacterDirectiveObjectiveModel, CharacterDirectiveObjective>(
                    () => _charactersDirectiveObjectiveCollection.GetCharacterDirectiveObjectivesAsync(characterId)),

                _storeUpdaterHelper.UpdateAsync<CensusCharacterDirectiveTreeModel, CharacterDirectiveTree>(
                    () => _charactersDirectiveTreeCollection.GetCharacterDirectiveTreesAsync(characterId)));

            var cacheKey = _getCharacterDirectiveCacheKey(characterId);
            await _cache.RemoveAsync(cacheKey);
        }
    }
}
