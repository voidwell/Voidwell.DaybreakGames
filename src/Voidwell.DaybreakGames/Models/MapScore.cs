using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class MapScore
    {
        public IEnumerable<int> Territories { get; set; }
        public IEnumerable<float> Percent { get; set; }
        public IEnumerable<int> ConnectedTerritories { get; set; }
        public IEnumerable<float> ConnectedPercent { get; set; }
    }
}
