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

        public async Task<Alert> GetActiveAlert(int worldId, int zoneId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Alerts
                    .AsNoTracking()
                    .OrderBy("StartDate", SortDirection.Descending)
                    .Where(a => a.MetagameEventId != 159 && a.MetagameEventId != 160 && a.MetagameEventId != 161 && a.MetagameEventId != 162)
                    .FirstOrDefaultAsync(a => a.WorldId == worldId && a.ZoneId == zoneId && a.EndDate == null);
            }
        }

        public async Task<IEnumerable<Alert>> GetAlertsByWorldId(int worldId, int limit)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = (from alert in dbContext.Alerts
                            join metagameEvent in dbContext.MetagameEventCategories on alert.MetagameEventId equals metagameEvent.Id into metagameEventQ
                            from metagameEvent in metagameEventQ.DefaultIfEmpty()
                            where alert.WorldId == worldId && alert.MetagameEventId != 159 && alert.MetagameEventId != 160 && alert.MetagameEventId != 161 && alert.MetagameEventId != 162
                            orderby alert.StartDate descending
                            select new { alert, metagameEvent })
                            .Take(limit);

                var result = query.ToList().Select(a =>
                {
                    a.alert.MetagameEvent = a.metagameEvent;
                    return a.alert;
                });

                return await Task.FromResult(result);
            }
        }

        public async Task<IEnumerable<Alert>> GetAllAlerts(int limit)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = (from alert in dbContext.Alerts
                            join metagameEvent in dbContext.MetagameEventCategories on alert.MetagameEventId equals metagameEvent.Id into metagameEventQ
                            from metagameEvent in metagameEventQ.DefaultIfEmpty()
                            where alert.MetagameEventId != 159 && alert.MetagameEventId != 160 && alert.MetagameEventId != 161 && alert.MetagameEventId != 162
                            orderby alert.StartDate descending
                            select new { alert, metagameEvent })
                            .Take(limit);

                var result = query.ToList().Select(a =>
                {
                    a.alert.MetagameEvent = a.metagameEvent;
                    return a.alert;
                });

                return await Task.FromResult(result);
            }
        }

        public async Task<Alert> GetAlert(int worldId, int instanceId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from alert in dbContext.Alerts
                            join metagameEvent in dbContext.MetagameEventCategories on alert.MetagameEventId equals metagameEvent.Id into metagameEventQ
                            from metagameEvent in metagameEventQ.DefaultIfEmpty()
                            where alert.WorldId == worldId && alert.MetagameInstanceId == instanceId
                            orderby alert.StartDate descending
                            select new { alert, metagameEvent };

                var result = query.ToList().Select(a =>
                {
                    a.alert.MetagameEvent = a.metagameEvent;
                    return a.alert;
                })
                .First();

                return await Task.FromResult(result);
            }
        }

        public async Task AddAsync(Alert entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                dbContext.Alerts.Add(entity);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Alert entity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

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
