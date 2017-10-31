using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/leaderboard")]
    public class LeaderboardController : Controller
    {
        private readonly IWeaponService _weaponService;

        public LeaderboardController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }

        [HttpGet("weapon/{weaponItemId}")]
        public async Task<ActionResult> GetWeaponLeaderboard(string weaponItemId)
        {
            var result = await _weaponService.GetLeaderboard(weaponItemId);
            return Ok(result);
        }
    }
}
