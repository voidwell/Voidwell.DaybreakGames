using System.Collections.Concurrent;

namespace Voidwell.DaybreakGames.Models
{
    public class WorldState
    {
        public WorldState()
        {
            ZoneStates = new ConcurrentDictionary<int, WorldZoneState>();
            OnlinePlayers = new ConcurrentDictionary<string, OnlineCharacter>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public ConcurrentDictionary<string, OnlineCharacter> OnlinePlayers { get; set; }
        public ConcurrentDictionary<int, WorldZoneState> ZoneStates { get; set;}
    }
}
