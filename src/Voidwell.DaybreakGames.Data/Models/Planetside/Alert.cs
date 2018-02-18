using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class Alert
    {
        [Required]
        public int WorldId { get; set; }
        [Required]
        public int MetagameInstanceId { get; set; }

        public int? ZoneId { get; set; }
        public int? MetagameEventId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public float? StartFactionVs { get; set; }
        public float? StartFactionNc { get; set; }
        public float? StartFactionTr { get; set; }
        public float? LastFactionVs { get; set; }
        public float? LastFactionNc { get; set; }
        public float? LastFactionTr { get; set; }

        public MetagameEventCategory MetagameEvent { get; set; }
    }
}
