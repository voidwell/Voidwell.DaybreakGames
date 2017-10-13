using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class MapScore
    {
        public IEnumerable<int> Territories { get; set; }
        public IEnumerable<double> Percent { get; set; }
        public IEnumerable<int> ConnectedTerritories { get; set; }
        public IEnumerable<double> ConnectedPercent { get; set; }
    }
}
