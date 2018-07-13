using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class AlertResult
    {
        public int WorldId { get; set; }
        public int MetagameInstanceId { get; set; }
        public int? ZoneId { get; set; }
        public int? MetagameEventId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public float StartFactionVS { get; set; }
        public float StartFactionNC { get; set; }
        public float StartFactionTR { get; set; }
        public float LastFactionVS { get; set; }
        public float LastFactionNC { get; set; }
        public float LastFactionTR { get; set; }
        public AlertResultMetagameEvent MetagameEvent { get; set; }
        public CombatReport Log { get; set; }
        public IEnumerable<float> Score { get; set; }
        public string ServerId { get; set; }
        public string MapId { get; set; }
        public IEnumerable<ZoneRegionOwnership> ZoneSnapshot { get; set; }
    }

    public class AlertResultMetagameEvent
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
