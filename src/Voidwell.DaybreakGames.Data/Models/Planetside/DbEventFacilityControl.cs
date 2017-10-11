using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("EventFacilityControl")]
    public class DbEventFacilityControl
    {
        [Required]
        public string FacilityId { get; set; }
        [Required]
        public string WorldId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public string ZoneId { get; set; }
        public string NewFactionId { get; set; }
        public string OldFactionId { get; set; }
        public int DurationHeld { get; set; }
        public string OutfitId { get; set; }
        public float ZoneControlVS { get; set; }
        public float ZoneControlNC { get; set; }
        public float ZoneControlTR { get; set; }
    }
}
