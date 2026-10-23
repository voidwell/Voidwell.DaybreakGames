using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

public interface ICharacterStore
{
    Task<IEnumerable<CensusCharacterModel>> LookupCharactersByName(string query, int limit = 12);
    Task<IEnumerable<Character>> FindCharacters(IEnumerable<long> characterIds);
    Task<Character?> GetCharacter(long characterId);
    Task UpdateAllCharacterInfo(long characterId, DateTime? lastLoginDate = null);
    Task<Character?> GetCharacterDetailsAsync(long characterId);
    Task<IEnumerable<Character>> GetCharacterDetailsAsync(IEnumerable<long> characterIds);
    Task<IEnumerable<CharacterWeaponStat>?> GetCharacterWeaponLeaderboardAsync(int weaponItemId, int page = 0, int limit = 50, string? sort = null, string? sortDir = null);
    Task<long?> GetCharacterIdByName(string characterName);
    Task<OutfitMember?> GetCharactersOutfitAsync(long characterId);
    Task<IEnumerable<CharacterAchievement>> GetCharacterAchievementsAsync(long characterId);
    Task<IEnumerable<CharacterWeaponStat>> GetWeaponStatsAsync(long characterId);
    Task UpdateCharacterAchievementsAsync(long characterId, DateTime? lastLoginDate = null);
}
