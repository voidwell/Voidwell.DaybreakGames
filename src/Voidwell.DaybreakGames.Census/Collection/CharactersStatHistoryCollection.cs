using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharactersStatHistoryCollection : CensusCollection
    {
        public override string CollectionName => "characters_stat_history";

        public CharactersStatHistoryCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusCharacterStatsHistoryModel>> GetCharacterStatsHistoryAsync(string characterId, DateTime? lastLogin = null)
        {
            return await QueryAsync(query =>
            {
                query.SetLimit(5000);
                query.ShowFields("character_id", "stat_name", "all_time", "one_life_max", "day", "month", "week");
                query.Where("character_id").Equals(characterId);

                if (lastLogin != null)
                {
                    query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
                }

                return query.GetBatchAsync<CensusCharacterStatsHistoryModel>();
            });
        }
    }
}