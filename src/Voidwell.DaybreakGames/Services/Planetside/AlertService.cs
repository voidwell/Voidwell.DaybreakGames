using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class AlertService : IAlertService
    {
        private readonly Func<PS2DbContext> _ps2DbContextFactory;
        private readonly ICombatReportService _combatReportService;

        public AlertService(Func<PS2DbContext> ps2DbContextFactory, ICombatReportService combatReportService)
        {
            _ps2DbContextFactory = ps2DbContextFactory;
            _combatReportService = combatReportService;
        }

        public async Task<IEnumerable<DbAlert>> GetAllAlerts(int limit = 25)
        {
            var dbContext = _ps2DbContextFactory();
            return await dbContext.Alerts.Include(i => i.MetagameEvent)
                .Include(i => i.MetagameEvent)
                .OrderBy("StartDate", SortDirection.Descending)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<IEnumerable<DbAlert>> GetAlerts(string worldId, int limit = 25)
        {
            var dbContext = _ps2DbContextFactory();
            return await dbContext.Alerts.Where(a => a.WorldId == worldId)
                .Include(i => i.MetagameEvent)
                .OrderBy("StartDate", SortDirection.Descending)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<AlertResult> GetAlert(string worldId, string instanceId)
        {
            var dbContext = _ps2DbContextFactory();
            var alert = await dbContext.Alerts.Where(a => a.WorldId == worldId && a.MetagameInstanceId == instanceId)
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
            var dbContext = _ps2DbContextFactory();
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
            dbContext.Alerts.Add(dataModel);
            return dbContext.SaveChangesAsync();
        }

        public async Task UpdateAlert(MetagameEvent metagameEvent)
        {
            var dbContext = _ps2DbContextFactory();
            var alert = await dbContext.Alerts
                    .AsTracking()
                    .SingleOrDefaultAsync(a => a.WorldId == metagameEvent.WorldId && a.MetagameInstanceId == metagameEvent.InstanceId);

            if (alert != null)
            {
                alert.EndDate = metagameEvent.Timestamp;
                alert.LastFactionVs = metagameEvent.FactionVs;
                alert.LastFactionNc = metagameEvent.FactionNc;
                alert.LastFactionTr = metagameEvent.FactionTr;

                dbContext.Alerts.Update(alert);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
