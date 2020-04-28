using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IWeaponService
    {
        Task<WeaponInfoResult> GetWeaponInfo(int weaponItemId);
        Task<WeaponInfoResult> GetWeaponInfoByName(string weaponName);
        Task<IEnumerable<WeaponLeaderboardRow>> GetLeaderboard(int weaponItemId, int page = 0, int limit = 50);
        Task<IEnumerable<int>> GetAllSanctionedWeaponIds();
        Task<Dictionary<int, IEnumerable<DailyWeaponStats>>> GetOracleStatsFromWeaponByDateAsync(IEnumerable<int> weaponId, DateTime start, DateTime end);
    }
}
