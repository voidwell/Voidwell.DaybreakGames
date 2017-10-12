using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

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
                StartFactionVS = alert.StartFactionVS,
                StartFactionNC = alert.StartFactionNC,
                StartFactionTR = alert.StartFactionTR,
                LastFactionVS = alert.LastFactionVS,
                LastFactionNC = alert.LastFactionNC,
                LastFactionTR = alert.LastFactionTR,
                MetagameEvent = new AlertResultMetagameEvent { Name = alert.MetagameEvent.Name, Description = alert.MetagameEvent.Description },
                Log = combatReport,
                Score = new[] { 0, alert.LastFactionVS, alert.LastFactionNC, alert.LastFactionTR },
                ServerId = alert.WorldId,
                MapId = alert.ZoneId
            };

            return alertResult;
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
