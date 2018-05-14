using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IWeaponService
    {
        Task<WeaponInfoResult> GetWeaponInfo(int weaponItemId);
        Task<IEnumerable<WeaponLeaderboardRow>> GetLeaderboard(int weaponItemId, string sortColumn = "Kills", SortDirection sortDirection = SortDirection.Descending, int rowStart = 0, int limit = 250);
        Task<IEnumerable<int>> GetAllSanctionedWeaponIds();
    }
}
