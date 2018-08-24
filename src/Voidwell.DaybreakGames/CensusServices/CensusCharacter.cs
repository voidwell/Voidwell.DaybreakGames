using DaybreakGames.Census;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.CensusServices
{
    public class CensusCharacter
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusCharacter(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<CensusCharacterModel> GetCharacter(string characterId)
        {
            var query = _queryFactory.Create("character");

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
                "title_id",
                "prestige_level"
            });
            query.Where("character_id").Equals(characterId);

            return await query.GetAsync<CensusCharacterModel>();
        }

        public async Task<string> GetCharacterIdByName(string characterName)
        {
            var query = _queryFactory.Create("character_name");

            query.Where("name.first_lower").Equals(characterName.ToLower());

            var result = await query.GetAsync<CensusCharacterModel>();

            return result?.CharacterId;
        }

        public async Task<CensusCharacterModel.CharacterTimes> GetCharacterTimes(string characterId)
        {
            var query = _queryFactory.Create("character");

            query.ShowFields(new[]
            {
                "character_id",
                "times.creation_date",
                "times.last_save_date",
                "times.last_login_date",
                "times.minutes_played"
            });
            query.Where("character_id").Equals(characterId);

            var result = await query.GetAsync<CensusCharacterModel>();
            return result?.Times;
        }

        public async Task<IEnumerable<CensusCharacterStatModel>> GetCharacterStats(string characterId, DateTime? lastLogin = null)
        {
            var query = _queryFactory.Create("characters_stat");

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
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
            }

            return await query.GetBatchAsync<CensusCharacterStatModel>();
        }

        public async Task<IEnumerable<CensusCharacterFactionStatModel>> GetCharacterFactionStats(string characterId, DateTime? lastLogin = null)
        {
            var query = _queryFactory.Create("characters_stat_by_faction");


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
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
            }

            return await query.GetBatchAsync<CensusCharacterFactionStatModel>();
        }

        public async Task<IEnumerable<CensusCharacterWeaponStatModel>> GetCharacterWeaponStats(string characterId, DateTime? lastLogin = null)
        {
            var query = _queryFactory.Create("characters_weapon_stat");

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
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
            }

            return await query.GetBatchAsync<CensusCharacterWeaponStatModel>();
        }

        public async Task<IEnumerable<CensusCharacterWeaponFactionStatModel>> GetCharacterWeaponStatsByFaction(string characterId, DateTime? lastLogin = null)
        {
            var query = _queryFactory.Create("characters_weapon_stat_by_faction");

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
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
            }

            return await query.GetBatchAsync<CensusCharacterWeaponFactionStatModel>();
        }

        public async Task<IEnumerable<CensusCharacterStatsHistoryModel>> GetCharacterStatsHistory(string characterId, DateTime? lastLogin = null)
        {
            var query = _queryFactory.Create("characters_stat_history");

            query.SetLimit(5000);
            query.ShowFields(new[]
            {
                "character_id",
                "stat_name",
                "all_time",
                "one_life_max",
                "day",
                "month",
                "week"
            });
            query.Where("character_id").Equals(characterId);

            if (lastLogin != null)
            {
                query.Where("last_save_date").IsGreaterThanOrEquals(lastLogin.Value);
            }

            return await query.GetBatchAsync<CensusCharacterStatsHistoryModel>();
        }

        public async Task<CensusOutfitMemberModel> GetCharacterOutfitMembership(string characterId)
        {
            var query = _queryFactory.Create("outfit_member");

            query.ShowFields(new[]
            {
                "character_id",
                "outfit_id",
                "member_since_date",
                "rank",
                "rank_ordinal"
            });
            query.Where("character_id").Equals(characterId);

            return await query.GetAsync<CensusOutfitMemberModel>();
        }

        public async Task<IEnumerable<CensusCharacterModel>> LookupCharactersByName(string name, int limit)
        {
            var query = _queryFactory.Create("character");

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
            query.Where("battle_rank.value").IsGreaterThan(0);

            return await query.GetListAsync<CensusCharacterModel>();
        }
    }
}
