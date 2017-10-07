using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public static class CharacterService
    {
        public static Task<JToken> LookupCharactersByName(string query, int limit = 12)
        {
            return Character.LookupCharactersByName(query, limit);
        }

        public static async Task<CensusServices.Models.Character> GetCharacterById(string characterId)
        {
            var character = await Character.GetCharacter(characterId);

            return character;
        }
    }
}
