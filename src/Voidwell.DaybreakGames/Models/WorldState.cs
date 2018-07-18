using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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

        public Dictionary<int, ZonePopulation> GetZonePopulations()
        {
            if (ZoneStates == null)
            {
                return null;
            }

            return ZoneStates.Keys.ToDictionary(a => a, a => GetZonePopulation(a));
        }

        public ZonePopulation GetZonePopulation(int zoneId)
        {
            if (OnlinePlayers == null)
            {
                return new ZonePopulation();
            }

            var zonePlayers = OnlinePlayers.Values.Where(p => p.Character.LastSeen != null && p.Character.LastSeen.ZoneId == zoneId && (DateTime.UtcNow - p.Character.LastSeen.Timestamp <= TimeSpan.FromMinutes(5)));
            var vsPlayers = zonePlayers.Count(p => p.Character.FactionId == 1);
            var ncPlayers = zonePlayers.Count(p => p.Character.FactionId == 2);
            var trPlayers = zonePlayers.Count(p => p.Character.FactionId == 3);

            return new ZonePopulation(vsPlayers, ncPlayers, trPlayers);
        }
    }
}
