using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using System;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface ICharacterService
    {
        Task<IEnumerable<CharacterSearchResult>> LookupCharactersByName(string query, int limit = 12);
        Task<IEnumerable<Character>> FindCharacters(IEnumerable<string> characterIds);
        Task<Character> GetCharacter(string characterId);
        Task<CharacterDetails> GetCharacterDetails(string characterId);
        Task<OutfitMember> GetCharactersOutfit(string characterId);
        Task UpdateAllCharacterInfo(string characterId, DateTime? LastLoginDate = null);
        Task<IEnumerable<Data.Models.Planetside.PlayerSession>> GetSessions(string characterId);
        Task<Models.PlayerSession> GetSession(string characterId, int sessionId);
    }
}