using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharactersDirectiveObjectiveCollection : CensusCollection
    {
        public override string CollectionName => "characters_directive_objective";

        public CharactersDirectiveObjectiveCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusCharacterDirectiveObjectiveModel>> GetCharacterDirectiveObjectivesAsync(string characterId)
        {
            return await QueryAsync(query =>
            {
                query.Where("character_id").Equals(characterId);

                return query.GetBatchAsync<CensusCharacterDirectiveObjectiveModel>();
            });
        }
    }
}
