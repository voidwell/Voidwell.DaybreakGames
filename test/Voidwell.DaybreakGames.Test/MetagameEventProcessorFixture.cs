using AutoMapper;
using Moq;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Live.Mappers;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Test
{
    public class MetagameEventProcessorFixture
    {
        public IEventRepository EventRepository { get; set; }
        public IMetagameEventService MetagameEventService { get; set; }
        public IWorldMonitor WorldMonitor { get; set; }
        public IAlertRepository AlertRepository { get; set; }
        public IMapService MapService { get; set; }

        public MetagameEventProcessor CreateSut()
        {
            var mapper = new MapperConfiguration(a => a.AddProfile<CensusToDataMappingProfile>())
                .CreateMapper();

            return new MetagameEventProcessor(EventRepository, MetagameEventService, WorldMonitor, AlertRepository, MapService, mapper);
        }

        public void ResetFixture()
        {
            EventRepository = Mock.Of<IEventRepository>();
            MetagameEventService = Mock.Of<IMetagameEventService>();
            WorldMonitor = Mock.Of<IWorldMonitor>();
            AlertRepository = Mock.Of<IAlertRepository>();
            MapService = Mock.Of<IMapService>();
        }
    }
}
