using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(int zoneId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.FacilityLinks.Where(a => a.ZoneId == zoneId)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<MapHex>> GetMapHexsByZoneIdAsync(int zoneId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.MapHexs.Where(a => a.ZoneId == zoneId)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(IEnumerable<int> facilityIds)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.MapRegions.Where(a => facilityIds.Contains(a.FacilityId))
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.MapRegions.Where(a => a.ZoneId == zoneId)
                    .ToListAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<MapHex> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var dbSet = dbContext.MapHexs;

                var storeEntities = await dbSet.ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storeEntities.SingleOrDefault(a => a.MapRegionId == entity.MapRegionId && a.ZoneId == entity.ZoneId && a.XPos == entity.XPos && a.YPos == entity.YPos);
                    if (storeEntity == null)
                    {
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
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var dbSet = dbContext.MapRegions;

                var storeEntities = await dbSet.ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storeEntities.SingleOrDefault(a => a.Id == entity.Id && a.FacilityId == entity.FacilityId);
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
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var dbSet = dbContext.FacilityLinks;

                var storeEntities = await dbSet.ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storeEntities.SingleOrDefault(a => a.ZoneId == entity.ZoneId && a.FacilityIdA == entity.FacilityIdA && a.FacilityIdB == entity.FacilityIdB);
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
