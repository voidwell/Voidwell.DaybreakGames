using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.Api.Authentication;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/outfit")]
public class OutfitController : Controller
{
    private readonly IOutfitService _outfitService;

    public OutfitController(IOutfitService outfitService)
    {
        _outfitService = outfitService;
    }

    [HttpGet("{outfitId}")]
    public async Task<ActionResult> GetOutfitAsync(string outfitId)
    {
        var details = await _outfitService.GetOutfitDetails(outfitId);
        if (details == null)
        {
            return NotFound($"Unable to find outfit with id: '{outfitId}'");
        }

        return Ok(details);
    }

    [HttpGet("{outfitId}/members")]
    public async Task<ActionResult> GetOutfitMembersAsync(string outfitId)
    {
        var result = await _outfitService.GetOutfitMembers(outfitId);
        return Ok(result);
    }

    [Authorize(AuthConstants.Policies.Mutterblack)]
    [HttpGet("byalias/{outfitAlias}")]
    public async Task<ActionResult> GetOutfitByAliasAsync(string outfitAlias)
    {
        var result = await _outfitService.GetOutfitByAlias(outfitAlias);
        if (result == null)
        {
            return NotFound($"Unable to find stats with outfit: '{outfitAlias}'");
        }

        return Ok(result);
    }
}
