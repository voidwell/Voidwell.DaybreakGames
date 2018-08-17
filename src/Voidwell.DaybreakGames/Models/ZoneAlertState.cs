using System;

namespace Voidwell.DaybreakGames.Models
{
    public class ZoneAlertState
    {
        public ZoneAlertState(DateTime timestamp, int? instanceId, int? metagameEventType, int? metagameEventId, TimeSpan? duration)
        {
            Timestamp = timestamp;
            InstanceId = instanceId;
            MetagameEventType = metagameEventType;
            MetagameEventId = metagameEventId;
            Duration = duration?.Minutes;
        }

        public DateTime Timestamp { get; set; }
        public int? InstanceId { get; set; }
        public int? MetagameEventType { get; set; }
        public int? MetagameEventId { get; set; }
        public int? Duration { get; set; }
    }
}
