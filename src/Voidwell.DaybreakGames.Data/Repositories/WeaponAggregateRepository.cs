using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Data.Repositories;

public class WeaponAggregateRepository : IWeaponAggregateRepository
{
    private readonly IDbContextFactory<PS2DbContext> _dbContextFactory;

    public WeaponAggregateRepository(IDbContextFactory<PS2DbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<WeaponAggregate>> GetWeaponAggregatesByItemIds(params int[] itemIds)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.WeaponAggregates.Where(a => itemIds.Contains(a.ItemId)).ToListAsync();
    }

    public async Task<WeaponAggregate?> GetWeaponAggregateByVehicleId(int vehicleId)
    {
        using var dbContext = _dbContextFactory.CreateDbContext();
        return await dbContext.WeaponAggregates.FirstOrDefaultAsync(a => a.ItemId == 0 && a.VehicleId == vehicleId);
    }
}
