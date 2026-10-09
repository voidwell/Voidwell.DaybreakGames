using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class UpdaterSchedulerRepository : IUpdaterSchedulerRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public UpdaterSchedulerRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<UpdaterScheduler?> GetUpdaterHistoryByServiceNameAsync(string serviceName)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.UpdaterScheduler.SingleOrDefaultAsync(u => u.Id == serviceName);
    }

    public async Task UpsertAsync(UpdaterScheduler entity)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entity);
    }
}
