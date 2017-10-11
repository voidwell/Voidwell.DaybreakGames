using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusCharacter
    {
        public static async Task<CensusCharacterModel> GetCharacter(string characterId)
        {
            var query = new CensusQuery.Query("character");

            query.AddResolve("world");
            query.ShowFields(new[]
            {
                "character_id",
                "name.first",
                "faction_id",
                "world_id",
                "battle_rank.value",
                "battle_rank.percent_to_next",
                "certs.earned_points",
                "title_id"
            });
            query.Where("character_id").Equals(characterId);

            var url = query.GetUri();

            return await query.Get<CensusCharacterModel>();
        }

        public static async Task<CensusCharacterModel.CharacterTimes> GetCharacterTimes(string characterId)
        {
            var query = new CensusQuery.Query("character");

            query.ShowFields(new[]
            {
                "character_id",
                "times.creation_date",
                "times.last_save_date",
                "times.last_login_date",
                "times.minutes_played"
            });
            query.Where("character_id").Equals(characterId);

            return await query.Get<CensusCharacterModel.CharacterTimes>();
        }

        public static async Task<IEnumerable<CensusCharacterStatModel>> GetCharacterStats(string characterId, DateTime? lastLogin = null)
        {
            var query = new CensusQuery.Query("characters_stat");

            query.SetLimit(500);
            query.ShowFields(new[]
            {
                "character_id",
                "stat_name",
                "profile_id",
                "value_forever"
            });
            query.Where("character_id").Equals(characterId);

            if (lastLogin != null)
            {
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.ToString());
            }

            return await query.GetList<CensusCharacterStatModel>();
        }

        public static async Task<IEnumerable<CensusCharacterFactionStatModel>> GetCharacterFactionStats(string characterId, DateTime? lastLogin = null)
        {
            var query = new CensusQuery.Query("characters_stat_by_faction");

            query.SetLimit(500);
            query.ShowFields(new[]
            {
                "character_id",
                "stat_name",
                "profile_id",
                "value_forever_vs",
                "value_forever_nc",
                "value_forever_tr"
            });
            query.Where("character_id").Equals(characterId);

            if (lastLogin != null)
            {
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.ToString());
            }

            return await query.GetList<CensusCharacterFactionStatModel>();
        }

        public static async Task<IEnumerable<CensusCharacterWeaponStatModel>> GetCharacterWeaponStats(string characterId, DateTime? lastLogin = null)
        {
            var query = new CensusQuery.Query("characters_weapon_stat");

            query.SetLimit(5000);
            query.ShowFields(new[]
            {
                "character_id",
                "stat_name",
                "item_id",
                "vehicle_id",
                "value"
            });
            query.Where("character_id").Equals(characterId);

            if (lastLogin != null)
            {
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.ToString());
            }

            return await query.GetList<CensusCharacterWeaponStatModel>();
        }

        public static async Task<IEnumerable<CensusCharacterWeaponFactionStatModel>> GetCharacterWeaponStatsByFaction(string characterId, DateTime? lastLogin = null)
        {
            var query = new CensusQuery.Query("characters_weapon_stat_by_faction");

            query.SetLimit(5000);
            query.ShowFields(new[]
            {
                "character_id",
                "stat_name",
                "item_id",
                "vehicle_id",
                "value_vs",
                "value_nc",
                "value_tr"
            });
            query.Where("character_id").Equals(characterId);

            if (lastLogin != null)
            {
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.ToString());
            }

            return await query.GetList<CensusCharacterWeaponFactionStatModel>();
        }

        public static async Task<CensusOutfitMemberModel> GetCharacterOutfitMembership(string characterId)
        {
            var query = new CensusQuery.Query("outfit_member");

            query.ShowFields(new[]
            {
                "character_id",
                "outfit_id",
                "member_since_date",
                "rank",
                "rank_ordinal"
            });
            query.Where("character_id").Equals(characterId);

            return await query.Get<CensusOutfitMemberModel>();
        }

        public static async Task<IEnumerable<CensusCharacterModel>> LookupCharactersByName(string name, int limit)
        {
            var query = new CensusQuery.Query("character");

            query.SetLimit(limit);
            query.ExactMatchFirst = true;
            query.AddResolve(new[]
            {
                "online_status",
                "world"
            });
            query.ShowFields(new[]
            {
                "character_id",
                "name.first",
                "battle_rank.value",
                "faction_id",
                "times.last_login"
            });
            query.Where("name.first_lower").StartsWith(name.ToLower());
            query.Where("battle_rank.value").IsGreaterThan("0");

            return await query.GetList<CensusCharacterModel>();
        }
    }
}
