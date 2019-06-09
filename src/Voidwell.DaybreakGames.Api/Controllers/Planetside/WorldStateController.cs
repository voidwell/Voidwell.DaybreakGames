using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Api.Models;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Api.Controllers.Planetside
{
    [Route("ps2/worldState")]
    public class WorldStateController : Controller
    {
        private readonly IWorldMonitor _worldMonitor;

        public WorldStateController(IWorldMonitor worldMonitor)
        {
            _worldMonitor = worldMonitor;
        }

        [HttpGet]
        public async Task<ActionResult> GetWorldStates()
        {
            var result = await _worldMonitor.GetWorldStates();
            return Ok(result);
        }

        [HttpGet("{worldId}")]
        public async Task<ActionResult> GetWorldState(int worldId)
        {
            var result = await _worldMonitor.GetWorldState(worldId);
            return Ok(result);
        }

        [HttpGet("{worldId}/players")]
        public async Task<ActionResult> GetOnlinePlayers(int worldId)
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

        [HttpPost("{worldId}/zone")]
        public Task SetupWorldZones(int worldId)
        {
            return _worldMonitor.SetupWorldZones(worldId);
        }
    }
}
