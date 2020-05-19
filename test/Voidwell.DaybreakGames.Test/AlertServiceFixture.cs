using Microsoft.Extensions.Logging;
using Moq;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Test
{
    public class AlertServiceFixture
    {
        public IAlertRepository AlertRepository { get; set; }
        public IMetagameEventService MetagameEventService { get; set; }
        public ICombatReportService CombatReportService { get; set; }
        public IMapService MapService { get; set; }
        public IWorldMonitor WorldMonitor { get; set; }
        public ICache Cache { get; set; }
        private ILogger<AlertService> Logger { get; set; }

        public AlertService CreateSut()
        {
            return new AlertService(AlertRepository, MetagameEventService, CombatReportService, MapService, WorldMonitor, Cache, Logger);
        }

        public void ResetFixture()
        {
            AlertRepository = Mock.Of<IAlertRepository>();
            MetagameEventService = Mock.Of<IMetagameEventService>();
            CombatReportService = Mock.Of<ICombatReportService>();
            MapService = Mock.Of<IMapService>();
            WorldMonitor = Mock.Of<IWorldMonitor>();
            Cache = Mock.Of<ICache>();
            Logger = Mock.Of<ILogger<AlertService>>();
        }
    }
}
