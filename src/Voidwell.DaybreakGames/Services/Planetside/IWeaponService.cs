using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IWeaponService
    {
        Task<WeaponInfoResult> GetWeaponInfo(string weaponItemId);
        Task<IEnumerable<WeaponLeaderboardRow>> GetLeaderboard(string weaponItemId, string sortColumn = "Kills", SortDirection sortDirection = SortDirection.Descending, int rowStart = 0, int limit = 250);
    }
}
