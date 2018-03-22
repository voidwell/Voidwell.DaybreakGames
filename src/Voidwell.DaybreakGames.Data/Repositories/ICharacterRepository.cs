using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using static Voidwell.DaybreakGames.Data.Repositories.CharacterRepository;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface ICharacterRepository
    {
        Task<IEnumerable<Character>> GetCharactersByIdsAsync(IEnumerable<string> characterIds);
        Task<Character> GetCharacterAsync(string characterId);
        Task<Character> GetCharacterWithDetailsAsync(string characterId);
        Task<Character> UpsertAsync(Character entity);
        Task<CharacterTime> UpsertAsync(CharacterTime entity);
        Task<CharacterLifetimeStat> UpsertAsync(CharacterLifetimeStat entity);
        Task<CharacterLifetimeStatByFaction> UpsertAsync(CharacterLifetimeStatByFaction entity);
        Task<IEnumerable<CharacterStat>> UpsertRangeAsync(IEnumerable<CharacterStat> entities);
        Task<IEnumerable<CharacterStatByFaction>> UpsertRangeAsync(IEnumerable<CharacterStatByFaction> entities);
        Task<IEnumerable<CharacterWeaponStat>> UpsertRangeAsync(IEnumerable<CharacterWeaponStat> entities);
        Task<IEnumerable<CharacterWeaponStatByFaction>> UpsertRangeAsync(IEnumerable<CharacterWeaponStatByFaction> entities);
        Task<IEnumerable<CharacterWeaponStat>> GetCharacterWeaponLeaderboardAsync(int weaponItemId, string sortColumn, SortDirection sortDirection, int rowStart, int limit);
        Task<IEnumerable<WeaponAggregate>> GetWeaponAggregates();
    }
}
