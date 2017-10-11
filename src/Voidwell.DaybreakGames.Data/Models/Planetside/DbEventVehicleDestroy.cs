using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("EventVehicleDestroy")]
    public class DbEventVehicleDestroy
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string VehicleId { get; set; }
        [Required]
        public string AttackerCharacterId { get; set; }
        [Required]
        public string AttackerVehicleId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public string WorldId { get; set; }
        public string ZoneId { get; set; }
        public string AttackerLoadoutId { get; set; }
        public string AttackerWeaponId { get; set; }
        public string FacilityId { get; set; }
        public string FactionId { get; set; }
    }
}
