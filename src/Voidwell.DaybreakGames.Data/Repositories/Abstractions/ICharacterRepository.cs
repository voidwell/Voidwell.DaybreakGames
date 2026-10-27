using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions;

public interface ICharacterRepository
{
    Task<IEnumerable<Character>> GetCharactersByIdsAsync(IEnumerable<long> characterIds);
    Task<long?> GetCharacterIdByName(string characterName, IEnumerable<int>? worldIds = null);
    Task<IEnumerable<Character>> LookupCharactersByNameAsync(string query, int limit, IEnumerable<int>? worldIds = null);
    Task<Character?> GetCharacterAsync(long characterId);
    Task<Character?> GetCharacterWithDetailsAsync(long characterId);
    Task<IEnumerable<Character>> GetCharacterWithDetailsAsync(params long[] characterIds);
    Task<CharacterRating?> GetCharacterRatingAsync(long characterId);
    Task<IEnumerable<CharacterWeaponStat>> GetCharacterWeaponLeaderboardAsync(int weaponItemId, int page, int limit);
    Task<IEnumerable<CharacterRating>> GetCharacterRatingLeaderboardAsync(int limit);
    Task<IEnumerable<CharacterAchievement>> GetCharacterAchievementsAsync(long characterId);
    Task<IEnumerable<CharacterWeaponStat>> GetWeaponStatsAsync(long characterId);

    Task<Character> UpsertAsync(Character entity);
    Task<CharacterTime> UpsertAsync(CharacterTime entity);
    Task<CharacterLifetimeStat> UpsertAsync(CharacterLifetimeStat entity);
    Task<CharacterLifetimeStatByFaction> UpsertAsync(CharacterLifetimeStatByFaction entity);
    Task<CharacterRating> UpsertAsync(CharacterRating entity);
    Task<IEnumerable<CharacterStat>?> UpsertRangeAsync(IEnumerable<CharacterStat> entities);
    Task<IEnumerable<CharacterStatByFaction>?> UpsertRangeAsync(IEnumerable<CharacterStatByFaction> entities);
    Task<IEnumerable<CharacterWeaponStat>?> UpsertRangeAsync(IEnumerable<CharacterWeaponStat> entities);
    Task<IEnumerable<CharacterWeaponStatByFaction>?> UpsertRangeAsync(IEnumerable<CharacterWeaponStatByFaction> entities);
    Task<IEnumerable<CharacterStatHistory>> UpsertRangeAsync(IEnumerable<CharacterStatHistory> entities);
    Task<IEnumerable<CharacterAchievement>> UpsertRangeAsync(IEnumerable<CharacterAchievement> entities);
}
