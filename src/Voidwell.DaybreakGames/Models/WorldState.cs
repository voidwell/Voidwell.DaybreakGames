using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class WorldState
    {
        public WorldState()
        {
            ZoneStates = new Dictionary<string, WorldZoneState>();
            OnlinePlayers = new Dictionary<string, OnlineCharacter>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public Dictionary<string, OnlineCharacter> OnlinePlayers { get; set; }
        public Dictionary<string, WorldZoneState> ZoneStates { get; set;}
    }
}
