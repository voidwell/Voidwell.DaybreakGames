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
        public float StartFactionVs { get; set; }
        public float StartFactionNc { get; set; }
        public float StartFactionTr { get; set; }
        public float LastFactionVs { get; set; }
        public float LastFactionNc { get; set; }
        public float LastFactionTr { get; set; }

        [ForeignKey("MetagameEventId")]
        public DbMetagameEventCategory MetagameEvent { get; set; }
    }
}
