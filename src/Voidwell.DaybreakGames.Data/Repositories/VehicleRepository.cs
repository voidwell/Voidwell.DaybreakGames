using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly IDbContextHelper _dbContextHelper;

    public VehicleRepository(IDbContextHelper dbContextHelper)
    {
        _dbContextHelper = dbContextHelper;
    }

    public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
    {
        using (var factory = _dbContextHelper.GetFactory())
        {
            var dbContext = factory.GetDbContext();

            return await dbContext.Vehicles.Include(i => i.Faction)
                .ToListAsync();
        }
    }

    public async Task UpsertRangeAsync(IEnumerable<Vehicle> entities)
    {
        using (var factory = _dbContextHelper.GetFactory())
        {
            var dbContext = factory.GetDbContext();

            await dbContext.UpsertAsync(entities);
        }
    }

    public async Task UpsertRangeAsync(IEnumerable<VehicleFaction> entities)
    {
        using (var factory = _dbContextHelper.GetFactory())
        {
            var dbContext = factory.GetDbContext();

            await dbContext.UpsertAsync(entities);
        }
    }
}
