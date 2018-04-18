using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
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
        public IEnumerable<WorldOnlineState> GetWorldState()
        {
            return _worldMonitor.GetWorldStates();
        }

        [HttpGet("{worldId}/players")]
        public IEnumerable<OnlineCharacter> GetOnlinePlayers(int worldId)
        {
            return _worldMonitor.GetOnlineCharactersByWorld(worldId);
        }

        [HttpGet("{worldId}/zone/{zoneId}")]
        public Task<MapZone> GetWorldZoneState(int worldId, int zoneId)
        {
            return _worldMonitor.GetZoneMapState(worldId, zoneId);
        }

        [HttpPost("{worldId}/zone")]
        public Task SetupWorldZones(int worldId)
        {
            return _worldMonitor.SetupWorldZones(worldId);
        }
    }
}
