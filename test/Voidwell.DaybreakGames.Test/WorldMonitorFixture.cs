using Microsoft.Extensions.Logging;
using Moq;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Test
{
    public class WorldMonitorFixture
    {
        public IWorldEventsService WorldEventService { get; set; }
        public IZoneStore ZoneStore { get; set; }
        public IWorldService WorldService { get; set; }
        public IMapService MapService { get; set; }
        public IPlayerMonitor PlayerMonitor { get; set; }
        private ILogger<WorldMonitor> Logger { get; set; }

        public WorldMonitor CreateSut()
        {
            return new WorldMonitor(WorldEventService, ZoneStore, WorldService, MapService, PlayerMonitor, Logger);
        }

        public void ResetFixture()
        {
            WorldEventService = Mock.Of<IWorldEventsService>();
            ZoneStore = Mock.Of<IZoneStore>();
            WorldService = Mock.Of<IWorldService>();
            MapService = Mock.Of<IMapService>();
            PlayerMonitor = Mock.Of<IPlayerMonitor>();
            Logger = Mock.Of<ILogger<WorldMonitor>>();
        }
    }
}
