using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/map")]
    public class MapController : Controller
    {
        private readonly IWorldMonitor _worldMonitor;

        public MapController(IWorldMonitor worldMonitor)
        {
            _worldMonitor = worldMonitor;
        }

        [HttpGet("territory/{worldId}/{zoneId}")]
        public async Task<ActionResult> GetWorldScore(string worldId, string zoneId)
        {
            var result = _worldMonitor.GetTerritory(worldId, zoneId);
            return Ok(result);
        }

        [HttpGet("territory/{worldId}/{zoneId}/{endDate}")]
        public async Task<ActionResult> GetWorldScoreFromDate(string worldId, string zoneId, DateTime endDate)
        {
            var result = _worldMonitor.GetTerritoryFromDate(worldId, zoneId, endDate);
            return Ok(result);
        }
    }
}
