using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Api.Models
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
