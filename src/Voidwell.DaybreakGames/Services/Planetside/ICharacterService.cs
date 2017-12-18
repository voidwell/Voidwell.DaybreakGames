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
        Task<IEnumerable<DbCharacter>> FindCharacters(IEnumerable<string> characterIds);
        Task<DbCharacter> GetCharacter(string characterId);
        Task<DbCharacter> GetCharacterDetails(string characterId);
        Task<DbOutfitMember> GetCharactersOutfit(string characterId);
        Task UpdateCharacter(string characterId, DateTime? LastLoginDate = null);
        Task<IEnumerable<DbPlayerSession>> GetSessions(string characterId);
        Task<PlayerSession> GetSession(string characterId, string sessionId);
    }
}