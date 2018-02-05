using System;
using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside.Events
{
    public class VehicleDestroy
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int? VehicleId { get; set; }
        [Required]
        public string AttackerCharacterId { get; set; }
        [Required]
        public int? AttackerVehicleId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public int WorldId { get; set; }
        public int ZoneId { get; set; }
        public int? AttackerLoadoutId { get; set; }
        public int? AttackerWeaponId { get; set; }
        public int? FacilityId { get; set; }
        public int? FactionId { get; set; }
    }
}
