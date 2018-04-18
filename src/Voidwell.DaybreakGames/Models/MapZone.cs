using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class MapZone
    {
        public IEnumerable<ZoneRegionOwnership> Ownership { get; set; }
        public IEnumerable<ZoneRegionHex> Hexs { get; set; }
        public IEnumerable<ZoneRegionLink> Links { get; set; }
    }
}
