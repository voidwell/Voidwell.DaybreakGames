using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/leaderboard")]
    public class LeaderboardController : Controller
    {
        private readonly ICharacterService _characterService;

        public LeaderboardController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet("weapon/{weaponItemId}")]
        public async Task<ActionResult> GetWeaponLeaderboard(int weaponItemId, [FromQuery]int page = 0)
        {
            var result = await _characterService.GetCharacterWeaponLeaderboardAsync(weaponItemId, page);
            return Ok(result);
        }
    }
}
