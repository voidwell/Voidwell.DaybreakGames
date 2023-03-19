using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services
{
    public interface ICharacterStore
    {
        Task<IEnumerable<CensusCharacterModel>> LookupCharactersByName(string query, int limit = 12);
        Task<IEnumerable<Character>> FindCharacters(IEnumerable<string> characterIds);
        Task<Character> GetCharacter(string characterId);
        Task UpdateAllCharacterInfo(string characterId, DateTime? lastLoginDate = null);
        Task<Character> GetCharacterDetailsAsync(string characterId);
        Task<IEnumerable<Character>> GetCharacterDetailsAsync(IEnumerable<string> characterIds);
        Task<IEnumerable<CharacterWeaponStat>> GetCharacterWeaponLeaderboardAsync(int weaponItemId, int page = 0, int limit = 50, string sort = null, string sortDir = null);
        Task<string> GetCharacterIdByName(string characterName);
        Task<OutfitMember> GetCharactersOutfitAsync(string characterId);
        Task<IEnumerable<CharacterAchievement>> GetCharacterAchievementsAsync(string characterId);
        Task<IEnumerable<CharacterWeaponStat>> GetWeaponStatsAsync(string characterId);
        Task UpdateCharacterAchievementsAsync(string characterId, DateTime? lastLoginDate = null);
    }
}