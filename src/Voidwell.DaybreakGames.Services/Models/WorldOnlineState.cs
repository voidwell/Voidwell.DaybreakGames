using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Services.Models
{
    public class WorldOnlineState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public int OnlineCharacters { get; set; }
        public IEnumerable<WorldOnlineZoneState> ZoneStates { get; set; }
    }

    public class WorldOnlineZoneState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsTracking { get; set; }
        public ZoneLockState LockState { get; set; }
        public ZoneAlertState AlertState { get; set; }
        public PopulationPeriod Population { get; set; }
    }
}
