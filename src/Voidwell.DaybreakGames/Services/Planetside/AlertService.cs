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
        private readonly ICache _cache;

        private readonly string _cacheKey = "ps2.alert";
        private readonly TimeSpan _cacheAlertsExpiration = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _cacheAlertExpiration = TimeSpan.FromMinutes(10);

        public AlertService(IAlertRepository alertRepository, ICombatReportService combatReportService, ICache cache)
        {
            _alertRepository = alertRepository;
            _combatReportService = combatReportService;
            _cache = cache;
        }

        public async Task<IEnumerable<DbAlert>> GetAllAlerts(int limit = 25)
        {
            var cacheKey = $"{_cacheKey}_alerts";

            var alerts = await _cache.GetAsync<IEnumerable<DbAlert>>(cacheKey);
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

        public Task<IEnumerable<DbAlert>> GetAlerts(string worldId, int limit = 25)
        {
            return _alertRepository.GetAlertsByWorldId(worldId, limit);
        }

        public async Task<AlertResult> GetAlert(string worldId, string instanceId)
        {
            var cacheKey = $"{_cacheKey}_alert_{worldId}_{instanceId}";

            var alertResult = await _cache.GetAsync<AlertResult>(cacheKey);
            if (alertResult != null)
            {
                return alertResult;
            }

            var alert = await _alertRepository.GetAlert(worldId, instanceId);
            if (alert == null)
            {
                return null;
            }

            var combatReport = await _combatReportService.GetCombatReport(alert.WorldId, alert.ZoneId, alert.StartDate, alert.EndDate);
            if (combatReport == null)
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
                StartFactionVS = alert.StartFactionVs,
                StartFactionNC = alert.StartFactionNc,
                StartFactionTR = alert.StartFactionTr,
                LastFactionVS = alert.LastFactionVs,
                LastFactionNC = alert.LastFactionNc,
                LastFactionTR = alert.LastFactionTr,
                MetagameEvent = new AlertResultMetagameEvent { Name = alert.MetagameEvent.Name, Description = alert.MetagameEvent.Description },
                Log = combatReport,
                Score = new[] { 0, alert.LastFactionVs, alert.LastFactionNc, alert.LastFactionTr },
                ServerId = alert.WorldId,
                MapId = alert.ZoneId
            };

            await _cache.SetAsync(cacheKey, alertResult, _cacheAlertExpiration);

            return alertResult;
        }

        public Task CreateAlert(MetagameEvent metagameEvent)
        {
            var dataModel = new DbAlert
            {
                WorldId = metagameEvent.WorldId,
                ZoneId = metagameEvent.ZoneId,
                MetagameInstanceId = metagameEvent.InstanceId,
                MetagameEventId = metagameEvent.MetagameEventId,
                StartDate = metagameEvent.Timestamp,
                StartFactionVs = metagameEvent.FactionVs,
                StartFactionNc = metagameEvent.FactionNc,
                StartFactionTr = metagameEvent.FactionTr,
                LastFactionVs = metagameEvent.FactionVs,
                LastFactionNc = metagameEvent.FactionNc,
                LastFactionTr = metagameEvent.FactionTr
            };
            return _alertRepository.AddAsync(dataModel);
        }

        public async Task UpdateAlert(MetagameEvent metagameEvent)
        {
            var alert = await _alertRepository.GetActiveAlert(metagameEvent.WorldId, metagameEvent.InstanceId);

            if (alert != null)
            {
                alert.EndDate = metagameEvent.Timestamp;
                alert.LastFactionVs = metagameEvent.FactionVs;
                alert.LastFactionNc = metagameEvent.FactionNc;
                alert.LastFactionTr = metagameEvent.FactionTr;

                await _alertRepository.UpdateAsync(alert);
            }
        }
    }
}
