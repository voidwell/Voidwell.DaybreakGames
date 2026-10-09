using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class ZoneRepository : IZoneRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public ZoneRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Zone>> GetAllZonesAsync()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Zones.ToListAsync();
    }

    public async Task<IEnumerable<Zone>> GetZonesByIdsAsync(params int[] zoneIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Zones
            .Where(a => zoneIds.Contains(a.Id))
            .ToListAsync();
    }

    public async Task UpsertRangeAsync(IEnumerable<Zone> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }
}
