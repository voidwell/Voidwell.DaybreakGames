using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class WorldState
    {
        public WorldState()
        {
            ZoneStates = new Dictionary<int, WorldZoneState>();
            OnlinePlayers = new Dictionary<string, OnlineCharacter>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public Dictionary<string, OnlineCharacter> OnlinePlayers { get; set; }
        public Dictionary<int, WorldZoneState> ZoneStates { get; set;}
    }
}
