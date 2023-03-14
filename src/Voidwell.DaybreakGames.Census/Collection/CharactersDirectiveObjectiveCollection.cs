using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharactersDirectiveObjectiveCollection : ICensusCollection<CensusCharacterDirectiveObjectiveModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "characters_directive_objective";

        public CharactersDirectiveObjectiveCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusCharacterDirectiveObjectiveModel>> GetCharacterDirectiveObjectivesAsync(string characterId)
        {
            return await _client.CreateQuery(CollectionName)
                .Where("character_id", a => a.Equals(characterId))
                .GetBatchAsync<CensusCharacterDirectiveObjectiveModel>();
        }
    }
}
