using Microsoft.AspNetCore.Mvc;
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
    }
}
