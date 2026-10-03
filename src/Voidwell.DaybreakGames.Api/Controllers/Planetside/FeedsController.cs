using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/feeds")]
public class FeedsController : Controller
{
    private readonly IFeedService _feedService;

    public FeedsController(IFeedService feedService)
    {
        _feedService = feedService;
    }

    [HttpGet("news")]
    public async Task<ActionResult> GetNewsAsync()
    {
        var result = await _feedService.GetNewsFeed();
        return Ok(result);
    }

    [HttpGet("updates")]
    public async Task<ActionResult> GetUpdatesAsync()
    {
        var result = await _feedService.GetUpdateFeed();
        return Ok(result);
    }
}
