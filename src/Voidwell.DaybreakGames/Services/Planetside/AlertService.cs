using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data;
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

        public AlertService(IAlertRepository alertRepository, ICombatReportService combatReportService)
        {
            _alertRepository = alertRepository;
            _combatReportService = combatReportService;
        }

        public Task<IEnumerable<DbAlert>> GetAllAlerts(int limit = 25)
        {
            return _alertRepository.GetAllAlerts(limit);
        }

        public Task<IEnumerable<DbAlert>> GetAlerts(string worldId, int limit = 25)
        {
            return _alertRepository.GetAlertsByWorldId(worldId, limit);
        }

        public async Task<AlertResult> GetAlert(string worldId, string instanceId)
        {
            var alert = await _alertRepository.GetAlert(worldId, instanceId);

            var combatReport = await _combatReportService.GetCombatReport(alert.WorldId, alert.ZoneId, alert.StartDate, alert.EndDate);

            var alertResult = new AlertResult
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
