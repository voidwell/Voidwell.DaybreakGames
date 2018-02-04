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
            using (var dbContext = _dbContextHelper.Create())
            {
                return await dbContext.Worlds.ToListAsync();
            }
        }

        public async Task UpsertRangeAsync(IEnumerable<World> entities)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                foreach (var entity in entities)
                {
                    var storeEntity = await dbContext.Worlds.AsNoTracking().SingleOrDefaultAsync(a => a.Id == entity.Id);
                    if (storeEntity == null)
                    {
                        dbContext.Worlds.Add(entity);
                    }
                    else
                    {
                        storeEntity = entity;
                        dbContext.Worlds.Update(storeEntity);
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
