using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

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

        public async Task UpsertRangeAsync(IEnumerable<World> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                await dbContext.Worlds.UpsertRangeAsync(entities, (a, e) => a.Id == e.Id);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
