using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("CharacterStatByFaction")]
    public class DbCharacterStatByFaction
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string ProfileId { get; set; }

        public int? DominationCountVS { get; set; }
        public int? DominationCountNC { get; set; }
        public int? DominationCountTR { get; set; }
        public int? FacilityCaptureCountVS { get; set; }
        public int? FacilityCaptureCountNC { get; set; }
        public int? FacilityCaptureCountTR { get; set; }
        public int? KilledByVS { get; set; }
        public int? KilledByNC { get; set; }
        public int? KilledByTR { get; set; }
        public int? KillsVS { get; set; }
        public int? KillsNC { get; set; }
        public int? KillsTR { get; set; }
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

        [ForeignKey("CharacterId")]
        public DbCharacter Character { get; set; }
        public DbProfile Profile { get; set; }
    }
}
