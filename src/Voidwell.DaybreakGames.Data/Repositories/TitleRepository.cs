using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class TitleRepository : ITitleRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public TitleRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task UpdateRangeAsync(IEnumerable<DbTitle> entities)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.Titles;

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
