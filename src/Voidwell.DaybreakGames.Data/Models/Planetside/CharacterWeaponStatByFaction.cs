using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterWeaponStatByFaction
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int ItemId { get; set; }
        [Required]
        public int VehicleId { get; set; }

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

        public Item Item { get; set; }
        public Vehicle Vehicle { get; set; }

        public Character Character { get; set; }
    }
}
