using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voidwell.DaybreakGames.Api.Authentication;
using Voidwell.DaybreakGames.Live.GameState;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside;

[Route("ps2/worldState")]
public class WorldStateController : Controller
{
    private readonly IWorldMonitor _worldMonitor;

    public WorldStateController(IWorldMonitor worldMonitor)
    {
        _worldMonitor = worldMonitor;
    }

    [HttpGet]
    public async Task<ActionResult> GetWorldStatesAsync()
    {
        var result = await _worldMonitor.GetWorldStates();
        return Ok(result);
    }

    [HttpGet("{worldId}")]
    public async Task<ActionResult> GetWorldStateAsync(int worldId)
    {
        var result = await _worldMonitor.GetWorldState(worldId);
        return Ok(result);
    }

    [HttpGet("{worldId}/players")]
    public async Task<ActionResult> GetOnlinePlayersAsync(int worldId)
    {
        var result = await _worldMonitor.GetOnlineCharactersByWorld(worldId);
        return Ok(result);
    }

    [HttpGet("{worldId}/{zoneId}/map")]
    public ActionResult GetZoneOwnership(int worldId, int zoneId)
    {
        var result = _worldMonitor.GetZoneOwnership(worldId, zoneId);
        return Ok(result);
    }

    [Authorize(Roles = AuthConstants.Roles.Administrator)]
    [HttpPost("{worldId}/zone")]
    public Task SetupWorldZones(int worldId)
    {
        return _worldMonitor.SetupWorldZones(worldId);
    }
}
