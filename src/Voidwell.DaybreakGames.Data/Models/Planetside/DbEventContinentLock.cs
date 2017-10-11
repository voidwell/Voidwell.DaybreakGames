using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("EventContinentLock")]
    public class DbEventContinentLock
    {
        [Required]
        public string WorldId { get; set; }
        [Required]
        public string ZoneId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public string MetagameEventId { get; set; }
        public string TriggeringFaction { get; set; }
        public float PopulationVS { get; set; }
        public float PopulationNC { get; set; }
        public float PopulationTR { get; set; }
    }
}
