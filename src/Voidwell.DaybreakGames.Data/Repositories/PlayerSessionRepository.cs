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

    public async Task AddAsync(PlayerSession entity, CancellationToken cancellationToken = default)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.PlayerSessions.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PlayerSession?> GetPlayerSessionAsync(int sessionId, CancellationToken cancellationToken)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.PlayerSessions.FirstOrDefaultAsync(a => a.Id == sessionId, cancellationToken);
    }

    public async Task<IEnumerable<PlayerSession>> GetPlayerSessionsByCharacterIdAsync(string characterId, int limit, CancellationToken cancellationToken, int page = 0)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.PlayerSessions.Where(a => a.CharacterId == characterId && a.Duration > 300000)
            .OrderByDescending(a => a.LoginDate)
            .Skip(page * limit)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
    }
}
