﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.Microservice.EntityFramework;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class ZoneRepository : IZoneRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public ZoneRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<Zone>> GetAllZonesAsync()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Zones.ToListAsync();
            }
        }

        public async Task<IEnumerable<Zone>> GetZonesByIdsAsync(params int[] zoneIds)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Zones
                    .Where(a => zoneIds.Contains(a.Id))
                    .ToListAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<Zone> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.UpsertAsync(entities);
            }
        }
    }
}
