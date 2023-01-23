using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Voidwell.DaybreakGames.Domain.Models
{
    public class ZoneLockState
    {
        public ZoneLockState(DateTime timestamp, int? metagameEventId, int? triggeringFaction)
        {
            State = ZoneLockStateEnum.LOCKED;
            Timestamp = timestamp;
            MetagameEventId = metagameEventId;
            TriggeringFaction = triggeringFaction;
        }

        public ZoneLockState(DateTime timestamp)
        {
            State = ZoneLockStateEnum.UNLOCKED;
            Timestamp = timestamp;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ZoneLockStateEnum State { get; }
        public DateTime Timestamp { get; }
        public int? MetagameEventId { get; }
        public int? TriggeringFaction { get; }
    }

    public enum ZoneLockStateEnum
    {
        LOCKED,
        UNLOCKED
    }
}
