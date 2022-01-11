using System;

namespace Voidwell.DaybreakGames.Models
{
    public class ZoneAlertState
    {
        private static readonly TimeSpan _defaultMetagameDuration = TimeSpan.FromMinutes(90);

        public ZoneAlertState(DateTime timestamp, int? instanceId, ZoneMetagameEvent metagameEvent)
        {
            Timestamp = timestamp;
            InstanceId = instanceId;
            MetagameEvent = metagameEvent;
        }

        public DateTime Timestamp { get; set; }
        public int? InstanceId { get; set; }
        public int? MetagameEventId { get; set; }
        public ZoneMetagameEvent MetagameEvent { get; set; }

        public bool IsEventEnded()
        {
            var eventDuration = MetagameEvent?.Duration ?? _defaultMetagameDuration;
            return DateTime.UtcNow - Timestamp > eventDuration;
        }
    }
}
