﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class MetagameEventRepository : IMetagameEventRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public MetagameEventRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<MetagameEventCategory> GetMetagameEventCategory(int metagameEventId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.MetagameEventCategories.FirstOrDefaultAsync(a => a.Id == metagameEventId);
            }
        }

        public async Task<int?> GetMetagameCategoryZoneId(int metagameEventId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var categoryZone = await dbContext.MetagameEventCategoryZones.FirstOrDefaultAsync(a => a.MetagameEventCategoryId == metagameEventId);
                return categoryZone?.ZoneId;
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<MetagameEventCategory> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.MetagameEventCategories.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<MetagameEventState> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.MetagameEventStates.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
