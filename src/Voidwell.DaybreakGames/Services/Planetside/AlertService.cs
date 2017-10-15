using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class AlertService : IAlertService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly ICombatReportService _combatReportService;

        public AlertService(PS2DbContext ps2DbContext, ICombatReportService combatReportService)
        {
            _ps2DbContext = ps2DbContext;
            _combatReportService = combatReportService;
        }

        public async Task<IEnumerable<DbAlert>> GetAllAlerts(int limit = 25)
        {
            return await _ps2DbContext.Alerts.Include(i => i.MetagameEvent)
                .Include(i => i.MetagameEvent)
                .OrderBy("StartDate", SortDirection.Descending)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<DbAlert>> GetAlerts(string worldId, int limit = 25)
        {
            return await _ps2DbContext.Alerts.Where(a => a.WorldId == worldId)
                .Include(i => i.MetagameEvent)
                .OrderBy("StartDate", SortDirection.Descending)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<AlertResult> GetAlert(string worldId, string instanceId)
        {
            var alert = await _ps2DbContext.Alerts.Where(a => a.WorldId == worldId && a.MetagameInstanceId == instanceId)
                .Include(i => i.MetagameEvent)
                .FirstOrDefaultAsync();

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
            _ps2DbContext.Alerts.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        public async Task UpdateAlert(MetagameEvent metagameEvent)
        {
            var alert = await _ps2DbContext.Alerts
                    .AsTracking()
                    .SingleOrDefaultAsync(a => a.WorldId == metagameEvent.WorldId && a.MetagameInstanceId == metagameEvent.InstanceId);

            if (alert != null)
            {
                alert.EndDate = metagameEvent.Timestamp;
                alert.LastFactionVs = metagameEvent.FactionVs;
                alert.LastFactionNc = metagameEvent.FactionNc;
                alert.LastFactionTr = metagameEvent.FactionTr;

                _ps2DbContext.Alerts.Update(alert);
                await _ps2DbContext.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
