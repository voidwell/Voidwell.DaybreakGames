using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
    }
}
