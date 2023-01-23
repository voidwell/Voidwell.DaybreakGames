using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Domain.Models
{
    public class WorldZoneRegion : ZoneRegion
    {
        public List<WorldZoneRegion> Links { get; set; } = new List<WorldZoneRegion>();
    }
}
