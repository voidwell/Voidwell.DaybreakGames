using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class AlertService : IAlertService
    {
        private readonly IAlertRepository _alertRepository;
        private readonly ICombatReportService _combatReportService;
        private readonly IMapService _mapService;
        private readonly ICache _cache;

        private readonly string _cacheKey = "ps2.alert";
        private readonly TimeSpan _cacheAlertsExpiration = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _cacheAlertExpiration = TimeSpan.FromMinutes(5);

        public AlertService(IAlertRepository alertRepository, ICombatReportService combatReportService, IMapService mapService, ICache cache)
        {
            _alertRepository = alertRepository;
            _combatReportService = combatReportService;
            _mapService = mapService;
            _cache = cache;
        }

        public async Task<IEnumerable<Alert>> GetAllAlerts(int limit = 25)
        {
            var cacheKey = $"{_cacheKey}_alerts";

            var alerts = await _cache.GetAsync<IEnumerable<Alert>>(cacheKey);
            if (alerts != null)
            {
                return alerts;
            }

            alerts = await _alertRepository.GetAllAlerts(limit);

            if (alerts != null && alerts.Any())
            {
                await _cache.SetAsync(cacheKey, alerts, _cacheAlertsExpiration);
            }

            return alerts;
        }

        public Task<IEnumerable<Alert>> GetAlerts(int worldId, int limit = 25)
        {
            return _alertRepository.GetAlertsByWorldId(worldId, limit);
        }

        public async Task<AlertResult> GetAlert(int worldId, int instanceId)
        {
            var cacheKey = $"{_cacheKey}_alert_{worldId}_{instanceId}";

            var alertResult = await _cache.GetAsync<AlertResult>(cacheKey);
            if (alertResult != null)
            {
                return alertResult;
            }

            var alert = await _alertRepository.GetAlert(worldId, instanceId);
            if (alert == null || !alert.ZoneId.HasValue || !alert.StartDate.HasValue)
            {
                return null;
            }

            var combatReportTask = _combatReportService.GetCombatReport(alert.WorldId, alert.ZoneId.Value, alert.StartDate.Value, alert.EndDate);
            var zoneSnapshotTask = _mapService.GetZoneSnapshotByMetagameEvent(worldId, instanceId);

            await Task.WhenAll(combatReportTask, zoneSnapshotTask);

            if (combatReportTask.Result == null)
            {
                return null;
            }

            alertResult = new AlertResult
            {
                WorldId = alert.WorldId,
                ZoneId = alert.ZoneId,
                MetagameInstanceId = alert.MetagameInstanceId,
                MetagameEventId = alert.MetagameEventId,
                StartDate = alert.StartDate,
                EndDate = alert.EndDate,
                StartFactionVS = alert.StartFactionVs.GetValueOrDefault(),
                StartFactionNC = alert.StartFactionNc.GetValueOrDefault(),
                StartFactionTR = alert.StartFactionTr.GetValueOrDefault(),
                LastFactionVS = alert.LastFactionVs.GetValueOrDefault(),
                LastFactionNC = alert.LastFactionNc.GetValueOrDefault(),
                LastFactionTR = alert.LastFactionTr.GetValueOrDefault(),
                MetagameEvent = new AlertResultMetagameEvent { Name = alert.MetagameEvent.Name, Description = alert.MetagameEvent.Description },
                Log = combatReportTask.Result,
                Score = new[] {
                    100 - (alert.LastFactionVs.GetValueOrDefault() + alert.LastFactionVs.GetValueOrDefault() + alert.LastFactionTr.GetValueOrDefault()),
                    alert.LastFactionVs.GetValueOrDefault(),
                    alert.LastFactionNc.GetValueOrDefault(),
                    alert.LastFactionTr.GetValueOrDefault()
                },
                ServerId = alert.WorldId.ToString(),
                MapId = alert.ZoneId.ToString(),
                ZoneSnapshot = zoneSnapshotTask.Result?.Ownership
            };

            await _cache.SetAsync(cacheKey, alertResult, _cacheAlertExpiration);

            return alertResult;
        }

        public async Task CreateAlert(MetagameEvent metagameEvent)
        {
            if (metagameEvent.ZoneId == null)
            {
                metagameEvent.ZoneId = await _alertRepository.GetMetagameCategoryZoneId(metagameEvent.MetagameEventId);
            }

            var dataModel = new Alert
            {
                WorldId = metagameEvent.WorldId,
                ZoneId = metagameEvent.ZoneId,
                MetagameInstanceId = metagameEvent.InstanceId,
                MetagameEventId = metagameEvent.MetagameEventId,
                StartDate = metagameEvent.Timestamp,
                EndDate = metagameEvent.Timestamp.AddMinutes(45),
                StartFactionVs = metagameEvent.FactionVs,
                StartFactionNc = metagameEvent.FactionNc,
                StartFactionTr = metagameEvent.FactionTr,
                LastFactionVs = metagameEvent.FactionVs,
                LastFactionNc = metagameEvent.FactionNc,
                LastFactionTr = metagameEvent.FactionTr
            };

            await Task.WhenAll(_alertRepository.AddAsync(dataModel), CreateAlertZoneSnapshot(metagameEvent));
        }

        public async Task UpdateAlert(MetagameEvent metagameEvent)
        {
            var alert = await _alertRepository.GetAlert(metagameEvent.WorldId, metagameEvent.InstanceId);

            if (alert != null)
            {
                alert.EndDate = metagameEvent.Timestamp;
                alert.LastFactionVs = metagameEvent.FactionVs;
                alert.LastFactionNc = metagameEvent.FactionNc;
                alert.LastFactionTr = metagameEvent.FactionTr;

                await _alertRepository.UpdateAsync(alert);
            }
        }

        private async Task CreateAlertZoneSnapshot(MetagameEvent metagameEvent)
        {
            if (metagameEvent.ZoneId != null)
            {
                await _mapService.CreateZoneSnapshot(metagameEvent.WorldId, metagameEvent.ZoneId.Value, metagameEvent.Timestamp, metagameEvent.InstanceId);
            }
        }
    }
}
