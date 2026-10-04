using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.Api.Models;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/map")]
public class MapController : Controller
{
    private readonly IWorldMonitor _worldMonitor;
    private readonly IMapService _mapService;

    public MapController(IWorldMonitor worldMonitor, IMapService mapService)
    {
        _worldMonitor = worldMonitor;
        _mapService = mapService;
    }

    [HttpGet("{zoneId}")]
    public async Task<ActionResult> GetZoneMapAsync(int zoneId)
    {
        var result = await _mapService.GetZoneMapAsync(zoneId);
        return Ok(result);
    }

    [HttpGet("territory/{worldId}/{zoneId}")]
    public ActionResult GetWorldScore(int worldId, int zoneId)
    {
        var result = _worldMonitor.GetTerritory(worldId, zoneId);
        return Ok(result);
    }

    [HttpGet("population/{worldId}/{zoneId}")]
    public async Task<ActionResult> GetWorldPopulationAsync(int worldId, int zoneId)
    {
        var result = await _worldMonitor.GetZonePopulation(worldId, zoneId);
        return Ok(result);
    }

    [HttpPost("territory")]
    public async Task<ActionResult> GetWorldScoreFromDateAsync([FromBody] CombatReportRequest request)
    {
        var result = await _worldMonitor.GetTerritoryFromDate(request.WorldId, request.ZoneId, request.EndDate);
        return Ok(result);
    }

    [HttpPost("snapshot/create")]
    public async Task<ActionResult> PostCreateZoneSnapshotAsync([FromBody] SnapshotRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _mapService.CreateZoneSnapshot(request.WorldId!.Value, request.ZoneId!.Value, request.Timestamp);
        return NoContent();
    }

    [HttpPost("snapshot")]
    public async Task<ActionResult> PostGetZoneSnapshotAsync([FromBody] SnapshotRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mapService.GetZoneSnapshotByDateTime(request.WorldId!.Value, request.ZoneId!.Value, request.Timestamp!.Value);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

}
