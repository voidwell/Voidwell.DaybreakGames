using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("EventMetagameEvent")]
    public class DbEventMetagameEvent
    {
        [Required]
        public string MetagameId{ get; set; }

        public DateTime Timestamp { get; set; }
        public string WorldId { get; set; }
        public string ZoneId { get; set; }
        public string InstanceId { get; set; }
        public string MetagameEventId { get; set; }
        public string MetagameEventState { get; set; }
        public int ExperienceBonus { get; set; }
        public float ZoneControlVS { get; set; }
        public float ZoneControlNC { get; set; }
        public float ZoneControlTR { get; set; }
    }
}
