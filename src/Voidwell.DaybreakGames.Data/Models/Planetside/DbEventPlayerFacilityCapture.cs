using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("EventPlayerFacilityCapture")]
    public class DbEventPlayerFacilityCapture
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string FacilityId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public string WorldId { get; set; }
        public string ZoneId { get; set; }
        public string OutfitId { get; set; }
    }
}
