using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class Character
    {
        public static async Task<Models.Character> GetCharacter(string characterId)
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

            return await query.Get<Models.Character>();
        }

        public static async Task<Models.Character.CharacterTimes> GetCharacterTimes(string characterId)
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

            return await query.Get<Models.Character.CharacterTimes>();
        }

        public static async Task<JToken> GetCharacterStats(string characterId, DateTime lastLogin)
        {
            var query = new CensusQuery.Query("characters_stat");

            query.SetLimit(500);
            query.ShowFields(new[]
            {
                "character_id",
                "stat_name",
                "profile_id",
                "value_forever",
                "value_monthly",
                "value_weekly",
                "value_daily"
            });
            query.Where("character_id").Equals(characterId);

            if (lastLogin != null)
            {
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.ToString());
            }

            return await query.Get();
        }

        public static async Task<JToken> GetCharacterFactionStats(string characterId, DateTime lastLogin)
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
                "value_forever_tr",
                "value_monthly_vs",
                "value_monthly_nc",
                "value_monthly_tr",
                "value_weekly_vs",
                "value_weekly_nc",
                "value_weekly_tr",
                "value_daily_vs",
                "value_daily_nc",
                "value_daily_tr"
            });
            query.Where("character_id").Equals(characterId);

            if (lastLogin != null)
            {
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.ToString());
            }

            return await query.Get();
        }

        public static async Task<JToken> GetCharacterWeaponStats(string characterId, DateTime lastLogin)
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

            return await query.Get();
        }

        public static async Task<JToken> GetCharacterWeaponStatsByFaction(string characterId, DateTime lastLogin)
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

            return await query.Get();
        }

        public static async Task<JToken> GetCharacterOutfitMembership(string characterId)
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

            return (await query.Get()).First;
        }

        public static async Task<JToken> LookupCharactersByName(string name, int limit)
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

            return await query.Get();
        }
    }
}
