using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("CharacterWeaponStatByFaction")]
    public class DbCharacterWeaponStatByFaction
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string ItemId { get; set; }
        public string VehicleId { get; set; }

        public int? DamageGivenVS { get; set; }
        public int? DamageGivenNC { get; set; }
        public int? DamageGivenTR { get; set; }
        public int? DamageTakenByVS { get; set; }
        public int? DamageTakenByNC { get; set; }
        public int? DamageTakenByTR { get; set; }
        public int? HeadshotsVS { get; set; }
        public int? HeadshotsNC { get; set; }
        public int? HeadshotsTR { get; set; }
        public int? KilledByVS { get; set; }
        public int? KilledByNC { get; set; }
        public int? KilledByTR { get; set; }
        public int? KillsVS { get; set; }
        public int? KillsNC { get; set; }
        public int? KillsTR { get; set; }
        public int? VehicleKillsVS { get; set; }
        public int? VehicleKillsNC { get; set; }
        public int? VehicleKillsTR { get; set; }

        [ForeignKey("CharacterId")]
        public DbCharacter Character { get; set; }
        public DbItem Item { get; set; }
        public DbVehicle Vehicle { get; set; }
    }
}
