using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside.Events
{
    public class FacilityControl
    {
        [Required]
        public int FacilityId { get; set; }
        [Required]
        public int WorldId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public int ZoneId { get; set; }
        public int? NewFactionId { get; set; }
        public int? OldFactionId { get; set; }
        public int DurationHeld { get; set; }
        public string OutfitId { get; set; }
        public float? ZoneControlVs { get; set; }
        public float? ZoneControlNc { get; set; }
        public float? ZoneControlTr { get; set; }
    }
}
