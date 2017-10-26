using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models;

namespace Voidwell.DaybreakGames.Data.DBContext
{
    public static class DbContextExtensions
    {
        public static async Task UpsertAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity) where TEntity : class, IDbModel<TEntity>
        {
            var storeEntity = await dbSet.AsNoTracking().SingleOrDefaultAsync(entity.Predicate);
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

        public static async Task UpsertRangeAsync<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities) where TEntity : class, IDbModel<TEntity>
        {
            foreach(var entity in entities)
            {
                var storeEntity = await dbSet.AsNoTracking().SingleOrDefaultAsync(entity.Predicate);
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
        }
    }
}
