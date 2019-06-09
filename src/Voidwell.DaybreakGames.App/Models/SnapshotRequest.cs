using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Models
{
    public class SnapshotRequest
    {
        [Required]
        public int? ZoneId { get; set; }
        [Required]
        public int? WorldId { get; set; }
        [Required]
        public DateTime? Timestamp { get; set; }
    }
}
