using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside.Events
{
    public class MetagameEvent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MetagameId{ get; set; }

        public DateTime Timestamp { get; set; }
        public int WorldId { get; set; }
        public int ZoneId { get; set; }
        public int? InstanceId { get; set; }
        public int? MetagameEventId { get; set; }
        public int? MetagameEventState { get; set; }
        public int? ExperienceBonus { get; set; }
        public float? ZoneControlVs { get; set; }
        public float? ZoneControlNc { get; set; }
        public float? ZoneControlTr { get; set; }
    }
}
