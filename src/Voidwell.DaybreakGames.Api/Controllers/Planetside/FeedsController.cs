using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Services;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/feeds")]
    public class FeedsController : Controller
    {
        private readonly IFeedService _feedService;

        public FeedsController(IFeedService feedService)
        {
            _feedService = feedService;
        }

        [HttpGet("news")]
        public async Task<ActionResult> GetNews()
        {
            var result = await _feedService.GetNewsFeed();
            return Ok(result);
        }

        [HttpGet("updates")]
        public async Task<ActionResult> GetUpdates()
        {
            var result = await _feedService.GetUpdateFeed();
            return Ok(result);
        }
    }
}
