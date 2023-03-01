using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.Microservice.EntityFramework;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class WorldRepository : IWorldRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public WorldRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<World>> GetAllWorldsAsync()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Worlds.ToListAsync();
            }
        }

        public async Task<IEnumerable<DailyPopulation>> GetDailyPopulationsByWorldIdAsync(int worldId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.DailyPopulations
                    .Where(a => a.WorldId == worldId)
                    .ToListAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<World> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.UpsertAsync(entities);
            }
        }
    }
}
