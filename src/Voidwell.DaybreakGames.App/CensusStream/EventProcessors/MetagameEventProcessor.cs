using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStream;
using Voidwell.DaybreakGames.CensusStream.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.App.CensusStream.EventProcessors
{
    [CensusEventProcessor("MetagameEvent")]
    public class MetagameEventProcessor : IEventProcessor<MetagameEvent>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMetagameEventService _metagameEventService;
        private readonly IAlertService _alertService;

        public MetagameEventProcessor(IEventRepository eventRepository, IMetagameEventService metagameEventService, IAlertService alertService)
        {
            _eventRepository = eventRepository;
            _metagameEventService = metagameEventService;
            _alertService = alertService;
        }

        public async Task Process(MetagameEvent payload)
        {
            // Daybreak reset their instance_id counter
            payload.InstanceId = int.Parse($"{payload.InstanceId}18");

            var metagameCategory = await _metagameEventService.GetMetagameEvent(payload.MetagameEventId);

            var dataModel = new Data.Models.Planetside.Events.MetagameEvent
            {
                InstanceId = payload.InstanceId,
                MetagameEventId = payload.MetagameEventId,
                MetagameEventState = payload.MetagameEventState,
                ZoneControlVs = payload.FactionVs,
                ZoneControlNc = payload.FactionNc,
                ZoneControlTr = payload.FactionTr,
                ExperienceBonus = (int)payload.ExperienceBonus,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId ?? metagameCategory.ZoneId
            };

            await Task.WhenAll(_eventRepository.AddAsync(dataModel), _alertService.ProcessMetagameEvent(payload));
        }
    }
}
