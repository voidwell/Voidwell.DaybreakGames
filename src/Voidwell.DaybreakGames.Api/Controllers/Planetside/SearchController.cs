using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/search")]
public class SearchController : Controller
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("{category}/{query}")]
    public async Task<ActionResult> SearchAsync(string category, string query)
    {
        var results = await _searchService.SearchPlanetside(category, query);
        return Ok(results);
    }
}
