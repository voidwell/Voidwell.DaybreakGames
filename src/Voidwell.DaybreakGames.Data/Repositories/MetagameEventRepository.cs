using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class MetagameEventRepository : IMetagameEventRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public MetagameEventRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task UpsertRangeAsync(IEnumerable<DbMetagameEventCategory> entities)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.MetagameEventCategories;

                foreach (var entity in entities)
                {
                    var storeEntity = await dbSet.AsNoTracking().SingleOrDefaultAsync(a => a.Id == entity.Id);
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

        public async Task UpsertRangeAsync(IEnumerable<DbMetagameEventState> entities)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.MetagameEventStates;

                foreach (var entity in entities)
                {
                    var storeEntity = await dbSet.AsNoTracking().SingleOrDefaultAsync(a => a.Id == entity.Id);
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
