using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class LoadoutRepository : ILoadoutRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public LoadoutRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task<IEnumerable<Loadout>> GetAllLoadoutsAsync()
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Loadouts.ToListAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<Loadout> entities)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var dbSet = dbContext.Loadouts;

                foreach (var entity in entities)
                {
                    var storeEntity = await dbSet.SingleOrDefaultAsync(a => a.Id == entity.Id);
                    if (storeEntity == null)
                    {
                        dbSet.Add(entity);
                    }
                    else
                    {
                        storeEntity = entity;
                        dbSet.Update(storeEntity);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
