using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public class FactionRepository : IFactionRepository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public FactionRepository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        public async Task UpsertRangeAsync(IEnumerable<DbFaction> entities)
        {
            using (var dbContext = _dbContextHelper.Create())
            {
                var dbSet = dbContext.Factions;

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
