using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class MetagameEventRepository : IMetagameEventRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public MetagameEventRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<MetagameEventCategory?> GetMetagameEventCategory(int metagameEventId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.MetagameEventCategories.FirstOrDefaultAsync(a => a.Id == metagameEventId);
    }

    public async Task<int?> GetMetagameCategoryZoneId(int metagameEventId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var categoryZone = await dbContext.MetagameEventCategoryZones.FirstOrDefaultAsync(a => a.MetagameEventCategoryId == metagameEventId);
        return categoryZone?.ZoneId;
    }

    public async Task UpsertRangeAsync(IEnumerable<MetagameEventCategory> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }

    public async Task UpsertRangeAsync(IEnumerable<MetagameEventState> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }
}
