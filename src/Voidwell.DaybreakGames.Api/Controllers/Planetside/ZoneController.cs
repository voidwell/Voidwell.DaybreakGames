using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/zone")]
public class ZoneController : Controller
{
    private readonly IZoneStore _zoneStore;

    public ZoneController(IZoneStore zoneStore)
    {
        _zoneStore = zoneStore;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllZonesAsync()
    {
        var result = await _zoneStore.GetAllZones();
        return Ok(result);
    }
}
