using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class MapRepository : IMapRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public MapRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(string zoneId)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.FacilityLinks.Where(a => a.ZoneId == zoneId)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(IEnumerable<string> facilityIds)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.MapRegions.Where(a => facilityIds.Contains(a.FacilityId))
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(string zoneId)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.MapRegions.Where(a => a.ZoneId == zoneId)
                    .ToListAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<MapHex> entities)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.MapHexs;

                foreach (var entity in entities)
                {
                    var storeEntity = await dbSet.AsNoTracking().SingleOrDefaultAsync(a => a.MapRegionId == entity.MapRegionId && a.ZoneId == entity.ZoneId && a.XPos == entity.XPos && a.YPos == entity.YPos);
                    if (storeEntity == null)
                    {
                        entity.Id = Guid.NewGuid().ToString();
                        await dbSet.AddAsync(entity);
                    }
                    else
                    {
                        entity.Id = storeEntity.Id;
                        storeEntity = entity;
                        dbSet.Update(storeEntity);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<MapRegion> entities)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.MapRegions;

                foreach (var entity in entities)
                {
                    var storeEntity = await dbSet.AsNoTracking().SingleOrDefaultAsync(a => a.Id == entity.Id);
                    if (storeEntity == null)
                    {
                        dbSet.Add(entity);
                    }
                    else
                    {
                        storeEntity = entity;
                        dbSet.Update(storeEntity);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<FacilityLink> entities)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.FacilityLinks;

                foreach (var entity in entities)
                {
                    var storeEntity = await dbSet.AsNoTracking().SingleOrDefaultAsync(a => a.ZoneId == entity.ZoneId && a.FacilityIdA == entity.FacilityIdA && a.FacilityIdB == a.FacilityIdB);
                    if (storeEntity == null)
                    {
                        entity.Id = Guid.NewGuid().ToString();
                        dbSet.Add(entity);
                    }
                    else
                    {
                        entity.Id = storeEntity.Id;
                        storeEntity = entity;
                        dbSet.Update(storeEntity);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
