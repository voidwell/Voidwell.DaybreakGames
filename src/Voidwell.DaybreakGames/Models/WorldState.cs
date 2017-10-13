using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class WorldState
    {
        public WorldState()
        {
            ZoneStates = new Dictionary<string, WorldZoneState>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public Dictionary<string, WorldZoneState> ZoneStates { get;set;}
    }
}
