using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Voidwell.DaybreakGames.Data.Extensions;

public static class DbSetExtensions
{
    public static async Task<TEntity> UpsertAsync<TEntity>(this DbContext ctx, TEntity entity)
        where TEntity : class
    {
        await ctx.Upsert(entity)
            .RunAsync();

        return entity;
    }

    public static async Task<IEnumerable<TEntity>> UpsertAsync<TEntity>(this DbContext ctx, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        await ctx.UpsertRange(entities)
            .RunAsync();

        return entities;
    }

    //
    // Summary:
    //     If a match is found, only update properties that are not set to null in the new passed entity
    public static async Task<TEntity> UpsertWithoutNullPropertiesAsync<TEntity>(this DbContext dbContext, TEntity entity)
        where TEntity : class
    {
        if (entity == null)
        {
            return default!;
        }

        var keyProps = GetKeyProperties<TEntity>(dbContext);
        var parameter = Expression.Parameter(typeof(TEntity), "e");

        var predicateExpression = GetPredicateExpression(parameter, keyProps, entity);

        var dbSet = dbContext.Set<TEntity>();
        var storeEntity = await dbSet.FirstOrDefaultAsync(predicateExpression);

        TEntity result;

        if (storeEntity == null)
        {
            await dbSet.AddAsync(entity);
            result = entity;
        }
        else
        {
            result = PrepareEntityUpdate(dbSet, storeEntity, entity);
        }

        await dbContext.SaveChangesAsync();

        return result;
    }

    //
    // Summary:
    //     If a match is found, only update properties that are not set to null in the new passed entity
    public static async Task<IEnumerable<TEntity>?> UpsertRangeWithoutNullPropertiesAsync<TEntity>(this DbContext dbContext, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        if (!entities.Any())
        {
            return null;
        }

        if (entities.Count() == 1)
        {
            var returnValue = await dbContext.UpsertWithoutNullPropertiesAsync(entities.First());
            return new[] { returnValue };
        }

        var keyProps = GetKeyProperties<TEntity>(dbContext);
        var parameter = Expression.Parameter(typeof(TEntity), "e");

        var predicateExpression = GetRangePredicateExpression(parameter, keyProps, entities);

        var dbSet = dbContext.Set<TEntity>();
        var storedEntities = await dbSet.Where(predicateExpression).ToListAsync();

        var result = new List<TEntity>();
        var createdEntities = new List<TEntity>();

        foreach (var entity in entities)
        {
            var predExpr = GetPredicateExpression(parameter, keyProps, entity).Compile();
            var storeEntity = storedEntities.FirstOrDefault(predExpr);
            if (storeEntity == null)
            {
                createdEntities.Add(entity);
            }
            else
            {
                var preparedEntity = PrepareEntityUpdate(dbSet, storeEntity, entity);
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

    private static IReadOnlyList<IProperty> GetKeyProperties<TEntity>(DbContext dbContext)
        where TEntity : class
    {
        return dbContext.Model.FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties;
    }

    private static Expression<Func<TEntity, bool>> GetPredicateExpression<TEntity>(ParameterExpression paramExpr, IReadOnlyList<IProperty> keyProps, TEntity entity)
        where TEntity : class
    {
        var body = keyProps
                .Select((p, i) => Expression.Equal(
                    Expression.Property(Expression.Constant(entity), p.Name),
                    Expression.Property(paramExpr, p.Name)))
                .Aggregate(Expression.AndAlso);

        return Expression.Lambda<Func<TEntity, bool>>(body, paramExpr);
    }

    private static Expression<Func<TEntity, bool>> GetRangePredicateExpression<TEntity>(ParameterExpression paramExpr, IReadOnlyList<IProperty> keyProps, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        var body = entities.Select((entity, i) =>
            keyProps.Select((p, i) => Expression.Equal(
                    Expression.Property(Expression.Constant(entity), p.Name),
                    Expression.Property(paramExpr, p.Name)))
                .Aggregate(Expression.AndAlso))
            .Aggregate(Expression.Or);

        return Expression.Lambda<Func<TEntity, bool>>(body, paramExpr);
    }

    private static T PrepareEntityUpdate<T>(DbSet<T> dbSet, T target, T source) where T : class
    {
        foreach (var fromProp in typeof(T).GetProperties())
        {
            var toProp = typeof(T).GetProperty(fromProp.Name);
            var toValue = toProp!.GetValue(source, null);
            if (toValue != null)
            {
                fromProp.SetValue(target, toValue, null);
            }
        }

        dbSet.Update(target);
        return target;
    }
}
