using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharactersWeaponStatCollection : CensusCollection
    {
        public override string CollectionName => "characters_weapon_stat";

        public CharactersWeaponStatCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusCharacterWeaponStatModel>> GetCharacterWeaponStatsAsync(string characterId, DateTime? lastLogin = null)
        {
            return await QueryAsync(query =>
            {
                query.SetLimit(5000);
                query.ShowFields("character_id", "stat_name", "item_id", "vehicle_id", "value");
                query.Where("character_id").Equals(characterId);

                if (lastLogin != null)
                {
                    query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
                }

                return query.GetBatchAsync<CensusCharacterWeaponStatModel>();
            });
        }
    }
}