using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface ILeaderboardService
    {
        Task<IEnumerable<WeaponLeaderboardRow>> GetCharacterWeaponLeaderboardAsync(int weaponItemId, int page = 0, int limit = 50, string sort = "kills", string sortDir = "desc");
    }
}