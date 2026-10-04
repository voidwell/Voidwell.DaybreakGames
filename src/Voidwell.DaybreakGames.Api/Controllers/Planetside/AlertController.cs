using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/alert")]
public class AlertController : Controller
{
    private readonly IAlertService _alertService;

    public AlertController(IAlertService alertService)
    {
        _alertService = alertService;
    }

    [HttpGet("{worldId}/{instanceId}")]
    public async Task<ActionResult> GetAlertAsync(int worldId, int instanceId)
    {
        var result = await _alertService.GetAlert(worldId, instanceId);
        return Ok(result);
    }

    [HttpGet("alerts/{pageNumber}")]
    public async Task<ActionResult> GetAlertsAsync(int pageNumber, [FromQuery] int? worldId)
    {
        var result = await _alertService.GetAlerts(pageNumber, worldId);
        return Ok(result);
    }
}
