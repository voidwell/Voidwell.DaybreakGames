using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Data
{
    public class Repository
    {
        private readonly IDbContextHelper _dbContextHelper;

        public Repository(IDbContextHelper dbContextHelper)
        {
            _dbContextHelper = dbContextHelper;
        }

        protected async Task<TEntity> UpsertAsync<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> predicate, bool ignoreNullProperties = false) where TEntity : class
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var dbSet = dbContext.Set<TEntity>();

                var storeEntity = await dbSet.FirstOrDefaultAsync(predicate);

                if (storeEntity == null)
                {
                    await dbSet.AddAsync(entity);
                }
                else
                {
                    PrepareEntityUpdate(dbSet, storeEntity, entity, ignoreNullProperties);
                }

                await dbContext.SaveChangesAsync();

                return entity;
            }
        }

        protected async Task<IEnumerable<TEntity>> UpsertRangeAsync<TEntity>(IEnumerable<TEntity> entities, Expression<Func<TEntity, bool>> searchPredicate, Func<TEntity, TEntity, bool> matchPredicate, bool ignoreNullProperties = false) where TEntity : class
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var result = new List<TEntity>();
                var createdEntities = new List<TEntity>();

                var dbSet = dbContext.Set<TEntity>();

                var storedStats = await dbSet
                    .Where(searchPredicate)
                    .ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storedStats.FirstOrDefault(storedEntity =>  matchPredicate(storedEntity, entity));
                    if (storeEntity == null)
                    {
                        createdEntities.Add(entity);
                    }
                    else
                    {
                        var preparedEntity = PrepareEntityUpdate(dbSet, storeEntity, entity, ignoreNullProperties);
                        result.Add(preparedEntity);
                    }
                }

                if (createdEntities.Any())
                {
                    await dbSet.AddRangeAsync(createdEntities);
                    result.AddRange(createdEntities);
                }

                await dbContext.SaveChangesAsync();

                return result;
            }
        }

        private static T PrepareEntityUpdate<T>(DbSet<T> dbSet, T target, T source, bool ignoreNullProperties) where T : class
        {
            if (ignoreNullProperties)
            {
                AssignNonNullProperties(ref target, source);
            }
            else
            {
                target = source;
            }

            dbSet.Update(target);
            return target;
        }

        private static void AssignNonNullProperties<T>(ref T target, T source) where T : class
        {
            foreach (var fromProp in typeof(T).GetProperties())
            {
                var toProp = typeof(T).GetProperty(fromProp.Name);
                var toValue = toProp.GetValue(source, null);
                if (toValue != null)
                {
                    fromProp.SetValue(target, toValue, null);
                }
            }
        }
    }
}
