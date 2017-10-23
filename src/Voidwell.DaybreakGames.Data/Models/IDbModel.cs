using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Voidwell.DaybreakGames.Data.Models
{
    public interface IDbModel<T> where T : class
    {
        Expression<Func<T, bool>> Predicate { get; }
    }
}
