using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharactersStatByFactionCollection : ICensusCollection<CensusCharacterFactionStatModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "characters_stat_by_faction";

        public CharactersStatByFactionCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusCharacterFactionStatModel>> GetCharacterFactionStatsAsync(string characterId, DateTime? lastLogin = null)
        {
            var query =_client.CreateQuery(CollectionName)
                .SetLimit(500)
                .ShowFields("character_id", "stat_name", "profile_id", "value_forever_vs", "value_forever_nc", "value_forever_tr")
                .Where("character_id", a => a.Equals(characterId));

            if (lastLogin != null)
            {
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
            }

            return await query.GetBatchAsync<CensusCharacterFactionStatModel>();
        }
    }
}