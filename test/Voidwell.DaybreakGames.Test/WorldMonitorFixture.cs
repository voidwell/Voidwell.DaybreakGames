using Microsoft.Extensions.Logging;
using Moq;
using Voidwell.DaybreakGames.Core.Services.Planetside;
using Voidwell.DaybreakGames.GameState.Services;

namespace Voidwell.DaybreakGames.Test
{
    public class WorldMonitorFixture
    {
        public IWorldEventsService WorldEventService { get; set; }
        public IZoneService ZoneService { get; set; }
        public IWorldService WorldService { get; set; }
        public IMapService MapService { get; set; }
        public IPlayerMonitor PlayerMonitor { get; set; }
        private ILogger<WorldMonitor> Logger { get; set; }

        public WorldMonitor CreateSut()
        {
            return new WorldMonitor(WorldEventService, ZoneService, WorldService, MapService, PlayerMonitor, Logger);
        }

        public void ResetFixture()
        {
            WorldEventService = Mock.Of<IWorldEventsService>();
            ZoneService = Mock.Of<IZoneService>();
            WorldService = Mock.Of<IWorldService>();
            MapService = Mock.Of<IMapService>();
            PlayerMonitor = Mock.Of<IPlayerMonitor>();
            Logger = Mock.Of<ILogger<WorldMonitor>>();
        }
    }
}
