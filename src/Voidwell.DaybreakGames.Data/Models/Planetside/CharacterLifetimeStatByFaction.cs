using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterLifetimeStatByFaction
    {
        [Required]
        public string CharacterId { get; set; }

        public int? DominationCountVS { get; set; }
        public int? DominationCountNC { get; set; }
        public int? DominationCountTR { get; set; }
        public int? FacilityCaptureCountVS { get; set; }
        public int? FacilityCaptureCountNC { get; set; }
        public int? FacilityCaptureCountTR { get; set; }
        public int? RevengeCountVS { get; set; }
        public int? RevengeCountNC { get; set; }
        public int? RevengeCountTR { get; set; }
        public int? WeaponDamageGivenVS { get; set; }
        public int? WeaponDamageGivenNC { get; set; }
        public int? WeaponDamageGivenTR { get; set; }
        public int? WeaponDamageTakenByVS { get; set; }
        public int? WeaponDamageTakenByNC { get; set; }
        public int? WeaponDamageTakenByTR { get; set; }
        public int? WeaponHeadshotsVS { get; set; }
        public int? WeaponHeadshotsNC { get; set; }
        public int? WeaponHeadshotsTR { get; set; }
        public int? WeaponKilledByVS { get; set; }
        public int? WeaponKilledByNC { get; set; }
        public int? WeaponKilledByTR { get; set; }
        public int? WeaponKillsVS { get; set; }
        public int? WeaponKillsNC { get; set; }
        public int? WeaponKillsTR { get; set; }
        public int? WeaponVehicleKillsVS { get; set; }
        public int? WeaponVehicleKillsNC { get; set; }
        public int? WeaponVehicleKillsTR { get; set; }

        public Character Character { get; set; }
    }
}
