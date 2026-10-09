using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class ExperienceRepository : IExperienceRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public ExperienceRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Experience?> GetExperienceById(int experienceId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Experience.FirstOrDefaultAsync(a => a.Id == experienceId);
    }

    public async Task UpsertRangeAsync(IEnumerable<Experience> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }
}
