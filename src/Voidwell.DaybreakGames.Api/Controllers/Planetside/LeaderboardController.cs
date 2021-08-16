using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/leaderboard")]
    public class LeaderboardController : Controller
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet("weapon/{weaponItemId}")]
        public async Task<ActionResult> GetWeaponLeaderboard(int weaponItemId, [FromQuery]int page = 0)
        {
            var result = await _leaderboardService.GetCharacterWeaponLeaderboardAsync(weaponItemId, page);
            return Ok(result);
        }
    }
}
