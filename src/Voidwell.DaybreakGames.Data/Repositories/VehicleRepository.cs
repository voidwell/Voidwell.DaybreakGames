using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
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

                await dbContext.Vehicles.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<VehicleFaction> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.VehicleFactions.UpsertRangeAsync(entities, (a, e) => a.VehicleId == e.VehicleId && a.FactionId == e.FactionId);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
