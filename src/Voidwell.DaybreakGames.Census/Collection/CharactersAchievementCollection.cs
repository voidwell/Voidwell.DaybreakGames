using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharactersAchievementCollection : ICensusCollection<CensusCharacterAchievementModel>
    {
        private readonly ICensusClient _client; 

        public string CollectionName => "characters_achievement";

        public CharactersAchievementCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusCharacterAchievementModel>> GetCharacterAchievementsAsync(string characterId, DateTime? lastLogin = null)
        {
            var query = _client.CreateQuery(CollectionName)
                .SetLimit(5000)
                .Where("character_id", a => a.Equals(characterId));

            if (lastLogin != null)
            {
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
            }

            return await query.GetBatchAsync<CensusCharacterAchievementModel>();
        }
    }
}
