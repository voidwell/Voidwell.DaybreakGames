using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class ExperienceRepository : IExperienceRepository
{
    private readonly IDbContextHelper _dbContextHelper;

    public ExperienceRepository(IDbContextHelper dbContextHelper)
    {
        _dbContextHelper = dbContextHelper;
    }

    public async Task<Experience?> GetExperienceById(int experienceId)
    {
        using (var factory = _dbContextHelper.GetFactory())
        {
            var dbContext = factory.GetDbContext();

            return await dbContext.Experience.FirstOrDefaultAsync(a => a.Id == experienceId);
        }
    }

    public async Task UpsertRangeAsync(IEnumerable<Experience> entities)
    {
        using (var factory = _dbContextHelper.GetFactory())
        {
            var dbContext = factory.GetDbContext();

            await dbContext.UpsertAsync(entities);
        }
    }
}
