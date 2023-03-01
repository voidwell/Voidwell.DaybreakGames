using DaybreakGames.Census;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharacterNameCollection : CensusCollection
    {
        public override string CollectionName => "character_name";

        public CharacterNameCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<string> GetCharacterIdByNameAsync(string characterName)
        {
            var nameModel = await QueryAsync(query =>
            {
                query.Where("name.first_lower").Equals(characterName.ToLower());

                return query.GetAsync<CensusCharacterNameModel>();
            });

            return nameModel?.CharacterId;
        }
    }
}
