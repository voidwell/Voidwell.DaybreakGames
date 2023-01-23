using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharacterCollection : CensusCollection
    {
        public override string CollectionName => "character";

        public CharacterCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<CensusCharacterModel> GetCharacterAsync(string characterId)
        {
            return await QueryAsync(query =>
            {
                query.AddResolve("world");
                query.ShowFields("character_id", "name.first", "faction_id", "world_id", "battle_rank.value", "battle_rank.percent_to_next", "certs.earned_points", "title_id", "prestige_level");
                query.Where("character_id").Equals(characterId);

                return query.GetAsync<CensusCharacterModel>();
            });
        }

        public async Task<CensusCharacterModel.CharacterTimes> GetCharacterTimesAsync(string characterId)
        {
            var result = await QueryAsync(query =>
            {
                query.ShowFields("character_id", "times.creation_date", "times.last_save_date", "times.last_login_date", "times.minutes_played");
                query.Where("character_id").Equals(characterId);

                return query.GetAsync<CensusCharacterModel>();
            });
            
            return result?.Times;
        }

        public async Task<IEnumerable<CensusCharacterModel>> LookupCharactersByNameAsync(string name, int limit)
        {
            return await QueryAsync(query =>
            {
                query.SetLimit(limit);
                query.ExactMatchFirst = true;
                query.AddResolve("online_status", "world");
                query.ShowFields("character_id", "name.first", "battle_rank.value", "faction_id", "times.last_login");
                query.Where("name.first_lower").StartsWith(name.ToLower());
                query.Where("battle_rank.value").IsGreaterThan(0);

                return query.GetListAsync<CensusCharacterModel>();
            });
        }
    }
}
