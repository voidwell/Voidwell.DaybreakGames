using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/ranks")]
    public class RankingsController : Controller
    {
        private readonly ICharacterRatingService _characterRatingService;

        public RankingsController(ICharacterRatingService characterRatingService)
        {
            _characterRatingService = characterRatingService;
        }

        [HttpGet]
        public async Task<ActionResult> GetPlayerRanks()
        {
            var result = await _characterRatingService.GetRatingsLeaderboardAsync(1000);
            return Ok(result);
        }
    }
}
