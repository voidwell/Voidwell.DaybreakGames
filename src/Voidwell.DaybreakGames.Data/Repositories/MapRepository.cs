using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.Microservice.EntityFramework;

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

                return await dbContext.MapRegions.Where(a => a.FacilityId != null && facilityIds.Contains(a.FacilityId.Value))
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

        public async Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByMetagameEvent(int worldId, int metagameInstanceId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.ZoneOwnershipSnapshots.Where(a => a.WorldId == worldId && a.MetagameInstanceId == metagameInstanceId)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByDateTime(int worldId, int zoneId, DateTime timestamp)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.ZoneOwnershipSnapshots.Where(a => a.WorldId == worldId && a.ZoneId == zoneId && a.Timestamp == timestamp)
                    .ToListAsync();
            }
        }

        public async Task InsertRangeAsync(IEnumerable<ZoneOwnershipSnapshot> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                dbContext.ZoneOwnershipSnapshots.AddRange(entities);

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<MapHex> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.UpsertAsync(entities);
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<MapRegion> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.UpsertAsync(entities);
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<FacilityLink> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.UpsertAsync(entities);
            }
        }
    }
}
