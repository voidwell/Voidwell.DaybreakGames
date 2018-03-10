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

        protected DbContext GetDbContext()
        {
            return _dbContextHelper.GetFactory().GetDbContext();
        }

        protected T GetDbContext<T>() where T: DbContext
        {
            return _dbContextHelper.GetFactory().GetDbContext() as T;
        }

        protected async Task<TEntity> UpsertAsync<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> predicate, bool ignoreNullProperties = false) where TEntity : class
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var dbSet = dbContext.Set<TEntity>();

                var storeEntity = await dbSet.AsNoTracking()
                    .FirstOrDefaultAsync(predicate);

                await PrepareEntity(dbSet, storeEntity, entity, ignoreNullProperties);

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

                var dbSet = dbContext.Set<TEntity>();

                var storedStats = await dbSet.AsNoTracking()
                    .Where(searchPredicate)
                    .ToListAsync();

                foreach (var entity in entities)
                {
                    var storeEntity = storedStats.FirstOrDefault(storedEntity =>  matchPredicate(storedEntity, entity));

                    var preparedEntity = await PrepareEntity(dbSet, storeEntity, entity, ignoreNullProperties);
                    result.Add(preparedEntity);
                }

                await dbContext.SaveChangesAsync();

                return result;
            }
        }

        private static async Task<T> PrepareEntity<T>(DbSet<T> dbSet, T Target, T Source, bool ignoreNullProperties) where T : class
        {
            if (Target == null)
            {
                await dbSet.AddAsync(Source);
                return Source;
            }

            if (ignoreNullProperties)
            {
                AssignNonNullProperties(ref Target, Source);
            }
            else
            {
                Target = Source;
            }

            dbSet.Update(Target);
            return Target;
        }

        private static void AssignNonNullProperties<T>(ref T Target, T Source) where T : class
        {
            foreach (var fromProp in typeof(T).GetProperties())
            {
                var toProp = typeof(T).GetProperty(fromProp.Name);
                var toValue = toProp.GetValue(Source, null);
                if (toValue != null)
                {
                    fromProp.SetValue(Target, toValue, null);
                }
            }
        }
    }
}
