using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface ICharacterRepository
    {
        Task<IEnumerable<DbCharacter>> GetCharactersByIdsAsync(IEnumerable<string> characterIds);
        Task<DbCharacter> GetCharacterAsync(string characterId);
        Task<DbCharacter> GetCharacterWithDetailsAsync(string characterId);
        Task<DbCharacter> UpsertAsync(DbCharacter entity);
        Task<DbCharacterTime> UpsertAsync(DbCharacterTime entity);
        Task<IEnumerable<DbCharacterStat>> UpsertRangeAsync(IEnumerable<DbCharacterStat> entities);
        Task<IEnumerable<DbCharacterStatByFaction>> UpsertRangeAsync(IEnumerable<DbCharacterStatByFaction> entities);
        Task<IEnumerable<DbCharacterWeaponStat>> UpsertRangeAsync(IEnumerable<DbCharacterWeaponStat> entities);
        Task<IEnumerable<DbCharacterWeaponStatByFaction>> UpsertRangeAsync(IEnumerable<DbCharacterWeaponStatByFaction> entities);
        Task<IEnumerable<DbCharacterWeaponStat>> GetCharacterWeaponLeaderboardAsync(string weaponItemId, string sortColumn, SortDirection sortDirection, int rowStart, int limit);
    }
}
