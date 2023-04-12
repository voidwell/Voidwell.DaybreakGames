using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using System;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    [CensusEventProcessor("MetagameEvent")]
    public class MetagameEventProcessor : IEventProcessor<MetagameEvent>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMetagameEventService _metagameEventService;
        private readonly IWorldMonitor _worldMonitor;
        private readonly IAlertRepository _alertRepository;
        private readonly IMapService _mapService;
        private readonly IMapper _mapper;

        private enum METAGAME_EVENT_STATE
        {
            STARTED = 135,
            RESTARTED = 136,
            CANCELED = 137,
            ENDED = 138,
            XPCHANGE = 139
        }

        public MetagameEventProcessor(IEventRepository eventRepository, IMetagameEventService metagameEventService,
            IWorldMonitor worldMonitor, IAlertRepository alertRepository, IMapService mapService, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _metagameEventService = metagameEventService;
            _worldMonitor = worldMonitor;
            _alertRepository = alertRepository;
            _mapService = mapService;
            _mapper = mapper;
        }

        public async Task Process(MetagameEvent payload)
        {
            // Daybreak reset their instance_id counter
            payload.InstanceId = int.Parse($"{payload.InstanceId}18");

            var metagameCategory = await _metagameEventService.GetMetagameEvent(payload.MetagameEventId);

            var model = _mapper.Map<Data.Models.Planetside.Events.MetagameEvent>(payload);
            model.ZoneId ??= metagameCategory?.ZoneId;

            await Task.WhenAll(_eventRepository.AddAsync(model), ProcessMetagameEventStateAsync(payload));
        }

        private async Task ProcessMetagameEventStateAsync(MetagameEvent metagameEvent)
        {
            var eventState = Enum.Parse<METAGAME_EVENT_STATE>(metagameEvent.MetagameEventState);
            if (eventState == METAGAME_EVENT_STATE.STARTED || eventState == METAGAME_EVENT_STATE.RESTARTED)
            {
                await StartAlert(metagameEvent);
            }
            else if (eventState == METAGAME_EVENT_STATE.ENDED || eventState == METAGAME_EVENT_STATE.CANCELED)
            {
                await EndAlert(metagameEvent);
            }
        }

        private async Task StartAlert(MetagameEvent metagameEvent)
        {
            var category = await _metagameEventService.GetMetagameEvent(metagameEvent.MetagameEventId);

            metagameEvent.ZoneId ??= category?.ZoneId;

            if (metagameEvent.ZoneId != null)
            {
                var zoneAlert = new ZoneAlertState(metagameEvent.Timestamp, metagameEvent.InstanceId, category);
                _worldMonitor.UpdateZoneAlert(metagameEvent.WorldId, (int)metagameEvent.ZoneId, zoneAlert);
            }

            var model = _mapper.Map<Alert>(metagameEvent);
            model.EndDate = category?.Duration != null ? metagameEvent.Timestamp + category.Duration : model.EndDate;

            var tasks = new List<Task>
            {
                _alertRepository.AddAsync(model)
            };

            if (model.ZoneId != Constants.KoltyrZoneId)
            {
                tasks.Add(CreateAlertZoneSnapshot(metagameEvent));
            }

            await Task.WhenAll(tasks);
        }

        private async Task EndAlert(MetagameEvent metagameEvent)
        {
            var alert = await _alertRepository.GetAlert(metagameEvent.WorldId, metagameEvent.InstanceId);

            if (metagameEvent.ZoneId != null || alert?.ZoneId != null)
            {
                var zoneId = metagameEvent.ZoneId ?? alert.ZoneId;
                _worldMonitor.UpdateZoneAlert(metagameEvent.WorldId, (int)zoneId);
            }

            if (alert == null)
            {
                return;
            }

            alert.EndDate = metagameEvent.Timestamp;
            alert.LastFactionVs = metagameEvent.FactionVs;
            alert.LastFactionNc = metagameEvent.FactionNc;
            alert.LastFactionTr = metagameEvent.FactionTr;

            await _alertRepository.UpdateAsync(alert);
        }

        private async Task CreateAlertZoneSnapshot(MetagameEvent metagameEvent)
        {
            if (metagameEvent.ZoneId != null)
            {
                var zoneOwnership = await _worldMonitor.RefreshZoneOwnership(metagameEvent.WorldId, metagameEvent.ZoneId.Value);
                //var zoneOwnership = _worldMonitor.GetZoneOwnership(metagameEvent.WorldId, metagameEvent.ZoneId.Value);
                if (zoneOwnership != null && zoneOwnership.Any())
                {
                    await _mapService.CreateZoneSnapshot(metagameEvent.WorldId, metagameEvent.ZoneId.Value, metagameEvent.Timestamp, metagameEvent.InstanceId, zoneOwnership);
                }
            }
        }
    }
}
