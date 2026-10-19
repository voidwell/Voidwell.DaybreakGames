using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class WorldRepository : IWorldRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public WorldRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<World>> GetAllWorldsAsync()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Worlds.ToListAsync();
    }

    public async Task<IEnumerable<DailyPopulation>> GetDailyPopulationsByWorldIdAsync(int worldId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.DailyPopulations
            .Where(a => a.WorldId == worldId)
            .ToListAsync();
    }

    public async Task UpsertRangeAsync(IEnumerable<World> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }
}
