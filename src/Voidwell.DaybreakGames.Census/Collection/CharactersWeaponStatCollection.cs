using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharactersWeaponStatCollection : ICensusCollection<CensusCharacterWeaponStatModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "characters_weapon_stat";

        public CharactersWeaponStatCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<IEnumerable<CensusCharacterWeaponStatModel>> GetCharacterWeaponStatsAsync(string characterId, DateTime? lastLogin = null)
        {
            var query = _client.CreateQuery(CollectionName)
                .SetLimit(5000)
                .ShowFields("character_id", "stat_name", "item_id", "vehicle_id", "value")
                .Where("character_id", a => a.Equals(characterId));

            if (lastLogin != null)
            {
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
            }

            return await query.GetBatchAsync<CensusCharacterWeaponStatModel>();
        }
    }
}