using DaybreakGames.Census;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharacterNameCollection : ICensusCollection<CensusCharacterNameModel>
    {
        private readonly ICensusClient _client;

        public string CollectionName => "character_name";

        public CharacterNameCollection(ICensusClient censusClient)
        {
            _client = censusClient;
        }

        public async Task<string> GetCharacterIdByNameAsync(string characterName)
        {
            var nameModel = await _client.CreateQuery(CollectionName)
                .Where("name.first_lower", a => a.Equals(characterName.ToLower()))
                .GetAsync<CensusCharacterNameModel>();

            return nameModel?.CharacterId;
        }
    }
}
