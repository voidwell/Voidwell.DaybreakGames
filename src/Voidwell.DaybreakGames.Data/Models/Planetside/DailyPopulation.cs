using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class DailyPopulation
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int WorldId { get; set; }

        public int VsCount { get; set; }
        public int NcCount { get; set; }
        public int TrCount { get; set; }
        public int NsCount { get; set; }
        public int VsAvgPlayTime { get; set; }
        public int NcAvgPlayTime { get; set; }
        public int TrAvgPlayTime { get; set; }
        public int NsAvgPlayTime { get; set; }
        public int AvgPlayTime { get; set; }
    }

    public class DailyPopulationComparer : IEqualityComparer<DailyPopulation>
    {
        public bool Equals(DailyPopulation x, DailyPopulation y)
        {
            if (ReferenceEquals(x, y)) return true;

            return x != null && y != null & x.Date.Equals(y.Date);
        }

        public int GetHashCode(DailyPopulation obj)
        {
            return obj.Date.GetHashCode();
        }
    }
}
