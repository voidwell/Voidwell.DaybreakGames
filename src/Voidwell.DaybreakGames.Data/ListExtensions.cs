using System;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.DaybreakGames.Data
{
    public static class ListExtensions
    {
        public static void SetGroup<TOuter, TInner, TKey>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Action<TOuter, IEnumerable<TInner>> resultSelector)
        {
            outer = outer.GroupJoin(inner, outerKeySelector, innerKeySelector, (a, b) => { resultSelector(a, b); return a; }).ToList();
        }
    }
}
