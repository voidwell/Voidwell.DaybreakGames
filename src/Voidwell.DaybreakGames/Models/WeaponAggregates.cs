using System;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Data.Repositories.Models;

namespace Voidwell.DaybreakGames.Models
{
    public class WeaponAggregates
    {
        public Dictionary<string, WeaponAggregate> Aggregates { get; set; }
        public DateTime CalculatedDate { get; set; }
    }
}
