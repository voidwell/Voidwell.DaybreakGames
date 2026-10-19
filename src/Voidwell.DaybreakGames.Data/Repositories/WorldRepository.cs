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

    /// <summary>
    /// Upserts the given worlds and marks every other world inactive. Existing worlds keep their
    /// active state, so a world deactivated by hand stays inactive; new worlds are added as active.
    /// </summary>
    public async Task SyncWorldsAsync(IEnumerable<World> entities)
    {
        var worlds = entities.ToList();
        if (!worlds.Any())
        {
            // An empty response is more likely a census problem than every world being retired.
            return;
        }

        var worldIds = worlds.Select(a => a.Id).ToList();

        using var dbContext = _dbContextFactory.CreateDbContext();
        using var transaction = await dbContext.Database.BeginTransactionAsync();

        await dbContext.UpsertRange<World>(worlds)
            .On(a => a.Id)
            .WhenMatched((existing, incoming) => new World { Name = incoming.Name })
            .RunAsync();

        await dbContext.Worlds
            .Where(a => a.IsActive && !worldIds.Contains(a.Id))
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsActive, false));

        await transaction.CommitAsync();
    }
}
