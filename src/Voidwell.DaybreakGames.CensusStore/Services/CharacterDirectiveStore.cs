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
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public class CharacterDirectiveStore : ICharacterDirectiveStore
    {
        private readonly ICharacterDirectiveRepository _repository;
        private readonly CharactersDirectiveCollection _charactersDirectiveCollection;
        private readonly CharactersDirectiveObjectiveCollection _charactersDirectiveObjectiveCollection;
        private readonly CharactersDirectiveTierCollection _charactersDirectiveTierCollection;
        private readonly CharactersDirectiveTreeCollection _charactersDirectiveTreeCollection;
        private readonly ICharacterStore _characterStore;
        private readonly IStoreUpdaterHelper _storeUpdaterHelper;
        private readonly ICache _cache;

        private const string _cacheKey = "ps2.characterDirectiveStore";
        private readonly Func<string, string> _getCharacterDirectiveCacheKey = characterId => $"{_cacheKey}_directives_{characterId}";

        private readonly KeyedSemaphoreSlim _characterLock = new KeyedSemaphoreSlim();

        public CharacterDirectiveStore(
            ICharacterDirectiveRepository repository,
            CharactersDirectiveCollection charactersDirectiveCollection,
            CharactersDirectiveObjectiveCollection charactersDirectiveObjectiveCollection,
            CharactersDirectiveTierCollection charactersDirectiveTierCollection,
            CharactersDirectiveTreeCollection charactersDirectiveTreeCollection,
            ICharacterStore characterStore,
            IStoreUpdaterHelper storeUpdaterHelper,
            ICache cache)
        {
            _repository = repository;
            _charactersDirectiveCollection = charactersDirectiveCollection;
            _charactersDirectiveObjectiveCollection = charactersDirectiveObjectiveCollection;
            _charactersDirectiveTierCollection = charactersDirectiveTierCollection;
            _charactersDirectiveTreeCollection = charactersDirectiveTreeCollection;
            _characterStore = characterStore;
            _storeUpdaterHelper = storeUpdaterHelper;
            _cache = cache;
        }

        public async Task<IEnumerable<CharacterDirectiveTree>> GetCharacterDirectivesAsync(string characterId)
        {
            using (await _characterLock.WaitAsync(characterId))
            {
                var data = await GetCharacterDirectiveStoreDataAsync(characterId);
                if (data == null || !data.Any())
                {
                    try
                    {
                        await Task.WhenAll(UpdateCharacterDirectiveDataAsync(characterId), _characterStore.UpdateCharacterAchievementsAsync(characterId));
                        data = await GetCharacterDirectiveStoreDataAsync(characterId);
                    }
                    catch (CensusConnectionException)
                    {
                        return null;
                    }
                }

                return data;
            }
        }

        private async Task<IEnumerable<CharacterDirectiveTree>> GetCharacterDirectiveStoreDataAsync (string characterId)
        {
            var directiveTreesTask = _repository.GetDirectiveTreesAsync(characterId);
            var directiveTiersTask = _repository.GetDirectiveTiersAsync(characterId);
            var directivesTask = _repository.GetDirectivesAsync(characterId);
            var directiveObjectivesTask = _repository.GetDirectiveObjectivesAsync(characterId);

            await Task.WhenAll(directiveTreesTask, directiveTiersTask, directivesTask, directiveObjectivesTask);

            var directiveTrees = directiveTreesTask.Result;
            var directiveTiers = directiveTiersTask.Result;
            var directives = directivesTask.Result;
            var directiveObjectives = directiveObjectivesTask.Result;

            if (directiveTrees == null)
            {
                return null;
            }

            directives?.SetGroupJoin(directiveObjectives, a => a.DirectiveId, a => a.DirectiveId, a => a.CharacterDirectiveObjectives);
            directiveTiers.SetGroupJoin(directives, a => new { a.DirectiveTreeId, a.DirectiveTierId }, a => new { a.Directive.DirectiveTreeId, a.Directive.DirectiveTierId }, a => a.CharacterDirectives);
            directiveTrees.SetGroupJoin(directiveTiers, a => a.DirectiveTreeId, a => a.DirectiveTreeId, a => a.CharacterDirectiveTiers);

            return directiveTrees;
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
