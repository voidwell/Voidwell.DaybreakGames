using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Data.DBContext
{
    public static class DbContextExtensions
    {
        public static async Task<EntityEntry<TEntity>> UpsertAsync<TEntity>(this DbContext dbContext, TEntity entity) where TEntity : class
        {
            var storeEntity = await dbContext.FindAsync<TEntity>(entity);
            if (storeEntity != null)
            {
                storeEntity = entity;

                return dbContext.Update(storeEntity);
            }

            return dbContext.Add(entity);
        }

        public static async Task<EntityEntry<TEntity>> UpsertAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity) where TEntity : class
        {
            var storeEntity = await dbSet.FindAsync(entity);
            if (storeEntity != null)
            {
                storeEntity = entity;

                return dbSet.Update(storeEntity);
            }

            return dbSet.Add(entity);
        }

        public static async Task UpsertRangeAsync<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities) where TEntity : class
        {
            foreach(var entity in entities)
            {
                var storeEntity = await dbSet.FindAsync(entity);
                if (storeEntity != null)
                {
                    storeEntity = entity;

                    dbSet.Update(storeEntity);
                }

                dbSet.Add(entity);
            }
        }
    }
}
