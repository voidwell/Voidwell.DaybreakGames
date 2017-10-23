using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models;

namespace Voidwell.DaybreakGames.Data.DBContext
{
    public static class DbContextExtensions
    {
        public static async Task<EntityEntry<TEntity>> UpsertAsync<TEntity>(this DbSet<TEntity> dbSet, TEntity entity) where TEntity : class, IDbModel<TEntity>
        {
            var storeEntity = await dbSet.SingleOrDefaultAsync(entity.Predicate);
            if (storeEntity != null)
            {
                storeEntity = entity;

                return dbSet.Update(storeEntity);
            }

            return dbSet.Add(entity);
        }

        public static async Task UpsertRangeAsync<TEntity>(this DbSet<TEntity> dbSet, IEnumerable<TEntity> entities) where TEntity : class, IDbModel<TEntity>
        {
            foreach(var entity in entities)
            {
                var storeEntity = await dbSet.SingleOrDefaultAsync(entity.Predicate);
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
