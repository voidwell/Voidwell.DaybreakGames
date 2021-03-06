﻿using System;

namespace Voidwell.DaybreakGames.Models
{
    public class ZoneAlertState
    {
        public ZoneAlertState(DateTime timestamp, int? instanceId, ZoneMetagameEvent metagameEvent)
        {
            Timestamp = timestamp;
            InstanceId = instanceId;
            MetagameEvent = metagameEvent;
        }

        public DateTime Timestamp { get; set; }
        public int? InstanceId { get; set; }
        public ZoneMetagameEvent MetagameEvent { get; set; }
    }
}
