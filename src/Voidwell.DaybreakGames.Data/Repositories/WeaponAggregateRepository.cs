using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class WeaponAggregateRepository : IWeaponAggregateRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public WeaponAggregateRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<WeaponAggregate> GetWeaponAggregateByItemId(int itemId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.WeaponAggregates.FirstOrDefaultAsync(a => a.ItemId == itemId);
            }
        }

        public async Task<WeaponAggregate> GetWeaponAggregateByVehicleId(int vehicleId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.WeaponAggregates.FirstOrDefaultAsync(a => a.ItemId == 0 && a.VehicleId == vehicleId);
            }
        }
    }
}
