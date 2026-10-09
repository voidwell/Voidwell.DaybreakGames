using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class PlayerSessionRepository : IPlayerSessionRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public PlayerSessionRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task AddAsync(PlayerSession entity)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.PlayerSessions.AddAsync(entity);
        await dbContext.SaveChangesAsync();
    }

    public async Task<PlayerSession?> GetPlayerSessionAsync(int sessionId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.PlayerSessions.FirstOrDefaultAsync(a => a.Id == sessionId);
    }

    public async Task<IEnumerable<PlayerSession>> GetPlayerSessionsByCharacterIdAsync(string characterId, int limit, int page = 0)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.PlayerSessions.Where(a => a.CharacterId == characterId && a.Duration > 300000)
            .OrderByDescending(a => a.LoginDate)
            .Take(limit)
            .Skip(page * limit)
            .ToArrayAsync();
    }
}
