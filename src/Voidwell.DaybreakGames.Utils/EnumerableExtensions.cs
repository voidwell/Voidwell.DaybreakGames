using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Voidwell.DaybreakGames.Utils
{
    public static class EnumerableExtensions
    {
        public static void SetGroupJoin<TOuter, TInner, TKey>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Expression<Func<TOuter, IEnumerable<TInner>>> mapExpression)
        {
            if (inner == null)
            {
                return;
            }

            var propertyInfo = (PropertyInfo)((MemberExpression)mapExpression.Body).Member;
            outer = outer.GroupJoin(inner, outerKeySelector, innerKeySelector,
                (o, i) =>
                {
                    propertyInfo.SetValue(o, i.ToList(), null);
                    return o;
                }).ToList();
        }

        public static IEnumerable<TResult> SelectManyNotNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            return source.Where(a => selector(a) != null).SelectMany(a => selector(a));
        }
    }
}
