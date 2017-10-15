using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class FacilityControlChange
    {
        public WorldZoneRegion Region { get; set; }
        public IEnumerable<float> Territory { get; set; }
    }
}
