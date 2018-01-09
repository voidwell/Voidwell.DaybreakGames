using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public AlertRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<DbAlert> GetActiveAlert(string worldId, string zoneId)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.Alerts
                    .AsNoTracking()
                    .OrderBy("StartDate", SortDirection.Descending)
                    .FirstOrDefaultAsync(a => a.WorldId == worldId && a.ZoneId == zoneId && a.EndDate == null);
            }
        }

        public async Task<IEnumerable<DbAlert>> GetAlertsByWorldId(string worldId, int limit)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.Alerts.Where(a => a.WorldId == worldId)
                    .Include(i => i.MetagameEvent)
                    .OrderBy("StartDate", SortDirection.Descending)
                    .Take(limit)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<DbAlert>> GetAllAlerts(int limit)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.Alerts.Include(i => i.MetagameEvent)
                    .Include(i => i.MetagameEvent)
                    .OrderBy("StartDate", SortDirection.Descending)
                    .Take(limit)
                    .ToListAsync();
            }
        }

        public async Task<DbAlert> GetAlert(string worldId, string instanceId)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.Alerts.Where(a => a.WorldId == worldId && a.MetagameInstanceId == instanceId)
                    .Include(i => i.MetagameEvent)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task AddAsync(DbAlert entity)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                dbContext.Alerts.Add(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(DbAlert entity)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.Alerts;

                var storeEntity = await dbSet
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.MetagameInstanceId == entity.MetagameInstanceId && a.WorldId == entity.WorldId);

                storeEntity = entity;
                dbSet.Update(storeEntity);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
