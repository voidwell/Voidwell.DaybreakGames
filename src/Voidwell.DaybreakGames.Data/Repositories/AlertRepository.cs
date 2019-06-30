using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using System.Collections.Generic;
using System.Linq;
using System;

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

                var query = from alert in dbContext.Alerts

                    join metagameEvent in dbContext.MetagameEventCategories on alert.MetagameEventId equals metagameEvent.Id into metagameEventQ
                    from metagameEvent in metagameEventQ.DefaultIfEmpty()

                    where alert.WorldId == worldId && alert.EndDate > DateTime.UtcNow && alert.ZoneId == zoneId && metagameEvent.Type != 5
                    select new { alert, metagameEvent };

                var result = await query.FirstOrDefaultAsync();
                if (result != null)
                {
                    result.alert.MetagameEvent = result.metagameEvent;
                }

                return result?.alert;
            }
        }

        public async Task<IEnumerable<Alert>> GetActiveAlertsByWorldId(int worldId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from alert in dbContext.Alerts

                    join metagameEvent in dbContext.MetagameEventCategories on alert.MetagameEventId equals metagameEvent.Id into metagameEventQ
                    from metagameEvent in metagameEventQ.DefaultIfEmpty()

                    where alert.WorldId == worldId && alert.EndDate > DateTime.UtcNow && metagameEvent.Type != 5
                    select new { alert, metagameEvent };

                var results = await query.ToListAsync();
                results?.ForEach(a => a.alert.MetagameEvent = a.metagameEvent);

                return results?.Select(a => a.alert);
            }
        }

        public async Task<IEnumerable<Alert>> GetAlerts(int pageNumber, int limit, int? worldId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var query = from alert in dbContext.Alerts

                    join metagameEvent in dbContext.MetagameEventCategories on alert.MetagameEventId equals metagameEvent.Id into metagameEventQ
                    from metagameEvent in metagameEventQ.DefaultIfEmpty()

                    join metagameEventZone in dbContext.MetagameEventCategoryZones on alert.MetagameEventId equals metagameEventZone.MetagameEventCategoryId into metagameEventZoneQ
                    from metagameEventZone in metagameEventZoneQ.DefaultIfEmpty()

                    where metagameEvent.Type != 5
                    orderby alert.StartDate descending
                    select new { alert, metagameEvent, metagameEventZone };

                if (worldId != null)
                {
                    query = query.Where(a => a.alert.WorldId == worldId);
                }

                var result = query.Skip(pageNumber * limit).Take(limit).ToList().Select(a =>
                {
                    a.alert.MetagameEvent = a.metagameEvent;

                    if (a.metagameEventZone != null)
                    {
                        a.alert.ZoneId = a.alert.ZoneId ?? a.metagameEventZone.ZoneId;
                    }

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

                    join metagameEventZone in dbContext.MetagameEventCategoryZones on alert.MetagameEventId equals metagameEventZone.MetagameEventCategoryId into metagameEventZoneQ
                    from metagameEventZone in metagameEventZoneQ.DefaultIfEmpty()

                    where alert.WorldId == worldId && alert.MetagameInstanceId == instanceId
                    select new { alert, metagameEvent, metagameEventZone };

                var result = await query.FirstOrDefaultAsync();
                if (result == null)
                {
                    return null;
                }

                result.alert.MetagameEvent = result.metagameEvent;

                if (result.alert.ZoneId == null && result.metagameEventZone != null)
                {
                    result.alert.ZoneId = result.metagameEventZone.ZoneId;
                }

                return result.alert;
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
                    .FirstOrDefaultAsync(a => a.MetagameInstanceId == entity.MetagameInstanceId && a.WorldId == entity.WorldId);

                storeEntity = entity;
                dbSet.Update(storeEntity);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
