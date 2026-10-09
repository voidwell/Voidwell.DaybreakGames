using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class MapRepository : IMapRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public MapRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<FacilityLink>> GetFacilityLinksByZoneIdAsync(int zoneId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.FacilityLinks.Where(a => a.ZoneId == zoneId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MapHex>> GetMapHexsByZoneIdAsync(int zoneId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.MapHexs.Where(a => a.ZoneId == zoneId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MapRegion>> GetMapRegionsByFacilityIdsAsync(IEnumerable<int> facilityIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.MapRegions.Where(a => a.FacilityId != null && facilityIds.Contains(a.FacilityId.Value))
            .ToListAsync();
    }

    public async Task<IEnumerable<MapRegion>> GetMapRegionsByZoneIdAsync(int zoneId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.MapRegions.Where(a => a.ZoneId == zoneId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByMetagameEvent(int worldId, int metagameInstanceId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.ZoneOwnershipSnapshots.Where(a => a.WorldId == worldId && a.MetagameInstanceId == metagameInstanceId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ZoneOwnershipSnapshot>> GetZoneSnapshotByDateTime(int worldId, int zoneId, DateTime timestamp)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.ZoneOwnershipSnapshots.Where(a => a.WorldId == worldId && a.ZoneId == zoneId && a.Timestamp == timestamp)
            .ToListAsync();
    }

    public async Task InsertRangeAsync(IEnumerable<ZoneOwnershipSnapshot> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        dbContext.ZoneOwnershipSnapshots.AddRange(entities);

        await dbContext.SaveChangesAsync();
    }

    public async Task UpsertRangeAsync(IEnumerable<MapHex> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }

    public async Task UpsertRangeAsync(IEnumerable<MapRegion> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }

    public async Task UpsertRangeAsync(IEnumerable<FacilityLink> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }
}
