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
        Task<DbCharacter> GetCharacter(string characterId);
        Task<DbCharacter> GetCharacterFull(string characterId);
        Task<DbOutfitMember> GetCharactersOutfit(string characterId);
        Task UpdateCharacter(string characterId, DateTime? LastLoginDate = null);
    }
}
