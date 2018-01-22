using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
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
        public Models.MapScore GetWorldScore(string worldId, string zoneId)
        {
            return _worldMonitor.GetTerritory(worldId, zoneId);
        }

        [HttpPost("territory")]
        public async Task<ActionResult> GetWorldScoreFromDate([FromBody]CombatReportRequest request)
        {
            var result = await _worldMonitor.GetTerritoryFromDate(request.WorldId, request.ZoneId, request.EndDate);
            return Ok(result);
        }
    }
}
