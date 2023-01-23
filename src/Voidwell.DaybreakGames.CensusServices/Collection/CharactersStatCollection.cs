using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharactersStatCollection : CensusCollection
    {
        public override string CollectionName => "characters_stat";

        public CharactersStatCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusCharacterStatModel>> GetCharacterStatsAsync(string characterId, DateTime? lastLogin = null)
        {
            return await QueryAsync(query =>
            {
                query.SetLimit(500);
                query.ShowFields("character_id", "stat_name", "profile_id", "value_forever");
                query.Where("character_id").Equals(characterId);

                if (lastLogin != null)
                {
                    query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
                }

                return query.GetBatchAsync<CensusCharacterStatModel>();
            });
        }
    }
}
