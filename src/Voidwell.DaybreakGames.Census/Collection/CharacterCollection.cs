using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharacterCollection : ICensusCollection<CensusCharacterModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "character";

        public CharacterCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<CensusCharacterModel> GetCharacterAsync(string characterId)
        {
            return await _client.CreateQuery(CollectionName)
                .AddResolve("world")
                .ShowFields("character_id", "name.first", "faction_id", "world_id", "battle_rank.value", "battle_rank.percent_to_next", "certs.earned_points", "title_id", "prestige_level")
                .Where("character_id", a => a.Equals(characterId))
                .GetAsync<CensusCharacterModel>();
        }

        public async Task<CensusCharacterModel.CharacterTimes> GetCharacterTimesAsync(string characterId)
        {
            var result = await _client.CreateQuery(CollectionName)
                .ShowFields("character_id", "times.creation_date", "times.last_save_date", "times.last_login_date", "times.minutes_played")
                .Where("character_id", a => a.Equals(characterId))
                .GetAsync<CensusCharacterModel>();
            
            return result?.Times;
        }

        public async Task<IEnumerable<CensusCharacterModel>> LookupCharactersByNameAsync(string name, int limit)
        {
            return await _client.CreateQuery(CollectionName)
                .SetLimit(limit)
                .UseExactMatchFirst()
                .AddResolve("online_status", "world")
                .ShowFields("character_id", "name.first", "battle_rank.value", "faction_id", "times.last_login")
                .Where("name.first_lower", a => a.StartsWith(name.ToLower()))
                .Where("battle_rank.value", a => a.IsGreaterThan(0))
                .GetListAsync<CensusCharacterModel>();
        }
    }
}
