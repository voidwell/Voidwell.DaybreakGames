using System;

namespace Voidwell.DaybreakGames.Models
{
    public class ZoneLockState
    {
        public ZoneLockState(DateTime timestamp, int? metagameEventId, int? triggeringFaction)
        {
            Timestamp = timestamp;
            MetagameEventId = metagameEventId;
            TriggeringFaction = triggeringFaction;
        }

        public DateTime Timestamp { get; set; }
        public int? MetagameEventId { get; set; }
        public int? TriggeringFaction { get; set; }
    }
}
