using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class SanctionedWeaponsRepository : ISanctionedWeaponsRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public SanctionedWeaponsRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<SanctionedWeapon>> GetAllSanctionedWeapons()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.SanctionedWeapons
            .Where(a => a.Type == "i")
            .ToListAsync();
    }
}
