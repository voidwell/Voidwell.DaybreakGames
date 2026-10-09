using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class ProfileRepository : IProfileRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public ProfileRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Profile>> GetAllProfilesAsync()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Profiles.ToListAsync();
    }

    public async Task UpsertRangeAsync(IEnumerable<Profile> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var dbSet = dbContext.Profiles;

        foreach (var entity in entities)
        {
            var storeEntity = await dbSet.SingleOrDefaultAsync(a => a.Id == entity.Id);
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
