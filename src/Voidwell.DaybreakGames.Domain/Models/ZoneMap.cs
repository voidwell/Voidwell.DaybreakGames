using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Domain.Models
{
    public class ZoneMap
    {
        public IEnumerable<ZoneRegion> Regions { get; set; }
        public IEnumerable<ZoneLink> Links { get; set; }
        public IEnumerable<ZoneHex> Hexs { get; set; }
    }
}
