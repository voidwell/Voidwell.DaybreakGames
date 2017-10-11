using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("EventContinentUnkock")]
    public class DbEventContinentUnlock
    {
        [Required]
        public string WorldId { get; set; }
        [Required]
        public string ZoneId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public string MetagameEventId { get; set; }
        public string TriggeringFaction { get; set; }
    }
}
