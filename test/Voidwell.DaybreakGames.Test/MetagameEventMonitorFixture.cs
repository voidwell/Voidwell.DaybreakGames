using Microsoft.Extensions.Logging;
using Moq;
using Voidwell.DaybreakGames.Core.Services.Planetside;
using Voidwell.DaybreakGames.GameState.Services;
using Voidwell.DaybreakGames.Messaging;

namespace Voidwell.DaybreakGames.Test
{
    public class MetagameEventMonitorFixture
    {
        public IMapService MapService { get; set; }
        public IMetagameEventService MetagameEventService { get; set; }
        public IMessageService MessageService { get; set; }
        public IWorldMonitor WorldMonitor { get; set; }
        public IAlertService AlertService { get; set; }
        private ILogger<MetagameEventMonitor> Logger { get; set; }

        public MetagameEventMonitor CreateSut()
        {
            return new MetagameEventMonitor(MapService, MetagameEventService, MessageService, WorldMonitor, AlertService, Logger);
        }

        public void ResetFixture()
        {
            MapService = Mock.Of<IMapService>();
            MetagameEventService = Mock.Of<IMetagameEventService>();
            MessageService = Mock.Of<IMessageService>();
            WorldMonitor = Mock.Of<IWorldMonitor>();
            AlertService = Mock.Of<IAlertService>();
            Logger = Mock.Of<ILogger<MetagameEventMonitor>>();
        }
    }
}
