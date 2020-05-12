using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.GameState.CensusStream.Models;
using Voidwell.DaybreakGames.Core.Services.Planetside;
using Voidwell.DaybreakGames.Messaging;
using Voidwell.DaybreakGames.Messaging.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.Extensions.Logging;
using System.Linq;
using Voidwell.DaybreakGames.GameState.Models;

namespace Voidwell.DaybreakGames.GameState.Services
{
    public class MetagameEventMonitor : IMetagameEventMonitor
    {
        private readonly IMetagameEventService _metagameEventService;
        private readonly IMessageService _messageService;
        private readonly IWorldMonitor _worldMonitor;
        private readonly IMapService _mapService;
        private readonly IAlertService _alertService;
        private readonly ILogger<MetagameEventMonitor> _logger;

        private enum METAGAME_EVENT_STATE
        {
            STARTED = 135,
            RESTARTED = 136,
            CANCELED = 137,
            ENDED = 138,
            XPCHANGE = 139
        }

        public MetagameEventMonitor(IMapService mapService, IMetagameEventService metagameEventService, IMessageService messagingService,
            IWorldMonitor worldMonitor, IAlertService alertService, ILogger<MetagameEventMonitor> logger)
        {
            _mapService = mapService;
            _metagameEventService = metagameEventService;
            _messageService = messagingService;
            _worldMonitor = worldMonitor;
            _alertService = alertService;
            _logger = logger;
        }

        public async Task ProcessMetagameEvent(MetagameEvent metagameEvent)
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

            if (metagameEvent.ZoneId == null)
            {
                metagameEvent.ZoneId = category?.ZoneId;
            }

            if (metagameEvent.ZoneId != null)
            {
                var zoneAlert = new ZoneAlertState(metagameEvent.Timestamp, metagameEvent.InstanceId, category);
                _worldMonitor.UpdateZoneAlert(metagameEvent.WorldId, (int)metagameEvent.ZoneId, zoneAlert);
            }

            _logger.LogInformation("Starting alert {worldId}.{metagameInstanceId}", metagameEvent.WorldId, metagameEvent.InstanceId);

            var alertModel = new Alert
            {
                WorldId = metagameEvent.WorldId,
                ZoneId = metagameEvent.ZoneId,
                MetagameInstanceId = metagameEvent.InstanceId,
                MetagameEventId = metagameEvent.MetagameEventId,
                StartDate = metagameEvent.Timestamp,
                EndDate = metagameEvent.Timestamp + (category?.Duration ?? TimeSpan.FromMinutes(45)),
                StartFactionVs = metagameEvent.FactionVs,
                StartFactionNc = metagameEvent.FactionNc,
                StartFactionTr = metagameEvent.FactionTr,
                LastFactionVs = metagameEvent.FactionVs,
                LastFactionNc = metagameEvent.FactionNc,
                LastFactionTr = metagameEvent.FactionTr
            };

            var eventMessage = new AlertStartMessage
            {
                Timestamp = metagameEvent.Timestamp,
                WorldId = metagameEvent.WorldId,
                ZoneId = metagameEvent.ZoneId.GetValueOrDefault(),
                MetagameEventDescription = category?.Description,
                MetagameEventId = metagameEvent.MetagameEventId,
                MetagameInstanceId = metagameEvent.InstanceId,
                ScoreFactionVS = metagameEvent.FactionVs,
                ScoreFactionNC = metagameEvent.FactionNc,
                ScoreFactionTR = metagameEvent.FactionTr
            };

            await Task.WhenAll(_alertService.CreateAlert(alertModel), _messageService.PublishAlertEvent(metagameEvent.WorldId, metagameEvent.InstanceId, eventMessage));
            await CreateAlertZoneSnapshot(metagameEvent);
        }

        private async Task EndAlert(MetagameEvent metagameEvent)
        {
            var categoryTask = _metagameEventService.GetMetagameEvent(metagameEvent.MetagameEventId);
            var alertTask = _alertService.GetAlert(metagameEvent.WorldId, metagameEvent.InstanceId);

            await Task.WhenAll(categoryTask, alertTask);

            var category = categoryTask.Result;
            var alert = alertTask.Result;

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

            _logger.LogInformation("Ending alert {worldId}.{metagameInstanceId}", alert.WorldId, alert.MetagameInstanceId);

            var eventMessage = new AlertEndMessage
            {
                Timestamp = metagameEvent.Timestamp,
                WorldId = metagameEvent.WorldId,
                ZoneId = metagameEvent.ZoneId.GetValueOrDefault(),
                MetagameEventId = metagameEvent.MetagameEventId,
                MetagameInstanceId = metagameEvent.InstanceId,
                MetagameEventDescription = category?.Description,
                ScoreFactionVS = metagameEvent.FactionVs,
                ScoreFactionNC = metagameEvent.FactionNc,
                ScoreFactionTR = metagameEvent.FactionTr
            };

            await Task.WhenAll(_alertService.UpdateAlert(alert), _messageService.PublishAlertEvent(metagameEvent.WorldId, metagameEvent.InstanceId, eventMessage));
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
