using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions
{
    public interface ILeaderboardService
    {
        Task<IEnumerable<WeaponLeaderboardRow>> GetCharacterWeaponLeaderboardAsync(int weaponItemId, int page = 0, int limit = 50, string sort = "kills", string sortDir = "desc");
    }
}