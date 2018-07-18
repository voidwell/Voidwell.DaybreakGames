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
        private readonly IMapService _mapService;

        public MapController(IWorldMonitor worldMonitor, IMapService mapService)
        {
            _worldMonitor = worldMonitor;
            _mapService = mapService;
        }

        [HttpGet("{zoneId}")]
        public Task<ZoneMap> GetZoneMap(int zoneId)
        {
            return _mapService.GetZoneMap(zoneId);
        }

        [HttpGet("territory/{worldId}/{zoneId}")]
        public MapScore GetWorldScore(int worldId, int zoneId)
        {
            return _worldMonitor.GetTerritory(worldId, zoneId);
        }

        [HttpGet("population/{worldId}/{zoneId}")]
        public ZonePopulation GetWorldPopulation(int worldId, int zoneId)
        {
            return _worldMonitor.GetZonePopulation(worldId, zoneId);
        }

        [HttpPost("territory")]
        public async Task<ActionResult> GetWorldScoreFromDate([FromBody]CombatReportRequest request)
        {
            var result = await _worldMonitor.GetTerritoryFromDate(request.WorldId, request.ZoneId, request.EndDate);
            return Ok(result);
        }

        [HttpPost("snapshot/create")]
        public async Task<ActionResult> PostCreateZoneSnapshot([FromBody]SnapshotRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _mapService.CreateZoneSnapshot(request.WorldId.Value, request.ZoneId.Value, request.Timestamp);
            return NoContent();
        }

        [HttpPost("snapshot")]
        public async Task<ActionResult> PostGetZoneSnapshot([FromBody]SnapshotRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mapService.GetZoneSnapshotByDateTime(request.WorldId.Value, request.ZoneId.Value, request.Timestamp.Value);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
