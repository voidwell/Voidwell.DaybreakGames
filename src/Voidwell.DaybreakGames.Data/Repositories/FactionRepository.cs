using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class FactionRepository : IFactionRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public FactionRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Faction?> GetFactionByIdAsync(int factionId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Factions.FindAsync(factionId);
    }
}
