using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Api.Models
{
    public class OracleStat
    {
        public DateTime Period { get; set; }
        public float? Value { get; set; }
    }

    public class OracleStatComparer : IEqualityComparer<OracleStat>
    {
        public bool Equals(OracleStat x, OracleStat y)
        {
            if (ReferenceEquals(x, y)) return true;

            return x != null && y != null && x.Period.Equals(y.Period);
        }

        public int GetHashCode(OracleStat obj)
        {
            return obj.Period.GetHashCode();
        }
    }
}
