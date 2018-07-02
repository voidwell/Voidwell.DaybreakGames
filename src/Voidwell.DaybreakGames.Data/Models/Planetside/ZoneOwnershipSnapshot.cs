using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class ZoneOwnershipSnapshot
    {
        [Required]
        public DateTime Timestamp { get; set; }
        [Required]
        public int WorldId { get; set; }
        [Required]
        public int ZoneId { get; set; }
        public int? MetagameInstanceId { get; set; }
        public int RegionId { get; set; }
        public int FactionId { get; set; }
    }
}
