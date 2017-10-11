using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("Alert")]
    public class DbAlert
    {
        [Required]
        public string WorldId { get; set; }
        [Required]
        public string MetagameInstanceId { get; set; }

        public string ZoneId { get; set; }
        public string MetagameEventId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public float StartFactionVS { get; set; }
        public float StartFactionNC { get; set; }
        public float StartFactionTR { get; set; }
        public float LastFactionVS { get; set; }
        public float LastFactionNC { get; set; }
        public float LastFactionTR { get; set; }
    }
}
