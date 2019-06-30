using System;
using System.Linq;
using System.Linq.Expressions;

namespace Voidwell.DaybreakGames.Data
{
    public static class LinqExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string attribute, SortDirection direction)
        {
            return ApplyOrdering(query, attribute, direction, "OrderBy");
        }

        public static IQueryable<T> ThenBy<T>(this IQueryable<T> query, string attribute, SortDirection direction)
        {
            return ApplyOrdering(query, attribute, direction, "ThenBy");
        }

        private static IQueryable<T> ApplyOrdering<T>(IQueryable<T> query, string attribute, SortDirection direction, string orderMethodName)
        {
            try
            {
                if (direction == SortDirection.Descending) orderMethodName += "Descending";

                var t = typeof(T);

                var param = Expression.Parameter(t);
                var property = t.GetProperty(attribute);

                return query.Provider.CreateQuery<T>(
                    Expression.Call(
                        typeof(Queryable),
                        orderMethodName,
                        new[] { t, property.PropertyType },
                        query.Expression,
                        Expression.Quote(
                            Expression.Lambda(
                                Expression.Property(param, property),
                                param))
                    ));
            }
            catch (Exception) // Probably invalid input, you can catch specifics if you want
            {
                return query; // Return unsorted query
            }
        }
    }

    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}
