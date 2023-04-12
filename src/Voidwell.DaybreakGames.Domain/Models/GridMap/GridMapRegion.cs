using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Domain.Models.GridMap
{
    public class GridMapRegion
    {
        public int RegionId { get; set; }
        public IEnumerable<GridMapLink> Links { get; set; }
        public IEnumerable<GridMapVertex> Vertices { get; set; }
    }
}
