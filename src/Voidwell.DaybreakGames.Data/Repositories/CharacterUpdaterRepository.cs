using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class CharacterUpdaterRepository : ICharacterUpdaterRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public CharacterUpdaterRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddAsync(CharacterUpdateQueue entity)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        var storeEntity = await dbContext.CharacterUpdateQueue.FirstOrDefaultAsync(a => a.CharacterId == entity.CharacterId);
        if (storeEntity == null)
        {
            dbContext.CharacterUpdateQueue.Add(entity);
        }
        else
        {
            storeEntity.Timestamp = DateTime.UtcNow;
            dbContext.CharacterUpdateQueue.Update(storeEntity);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(CharacterUpdateQueue entity)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        dbContext.CharacterUpdateQueue.Remove(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<CharacterUpdateQueue>> GetAllAsync(TimeSpan? delay = null)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.CharacterUpdateQueue
            .Where(a => DateTime.UtcNow - a.Timestamp >= delay)
            .OrderBy(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<int> GetQueueLengthAsync(TimeSpan? delay = null)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        if (delay != null)
        {
            return await dbContext.CharacterUpdateQueue
                .Where(a => DateTime.UtcNow - a.Timestamp >= delay)
                .CountAsync();
        }

        return await dbContext.CharacterUpdateQueue
            .CountAsync();
    }
}
