using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public VehicleRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.Vehicles.Include(i => i.Faction)
            .ToListAsync();
    }

    public async Task UpsertRangeAsync(IEnumerable<Vehicle> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }

    public async Task UpsertRangeAsync(IEnumerable<VehicleFaction> entities)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        await dbContext.UpsertAsync(entities);
    }
}
