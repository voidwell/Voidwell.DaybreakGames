using System.Collections.Generic;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.GameState.Models
{
    public class WorldZoneRegion : ZoneRegion
    {
        public List<WorldZoneRegion> Links { get; set; } = new List<WorldZoneRegion>();
    }
}
