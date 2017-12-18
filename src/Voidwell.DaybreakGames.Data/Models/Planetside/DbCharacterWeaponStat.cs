using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("CharacterWeaponStat")]
    public class DbCharacterWeaponStat
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string ItemId { get; set; }
        public string VehicleId { get; set; }

        public int? Kills { get; set; }
        public int? VehicleKills { get; set; }
        public int? Deaths { get; set; }
        public int? KilledBy { get; set; }
        public int? FireCount { get; set; }
        public int? HitCount { get; set; }
        public int? Headshots { get; set; }
        public int? PlayTime { get; set; }
        public int? Score { get; set; }
        public int? DamageGiven { get; set; }
        public int? DamageTakenBy { get; set; }

        [ForeignKey("CharacterId")]
        public DbCharacter Character { get; set; }
        public DbItem Item { get; set; }
        public DbVehicle Vehicle { get; set; }
    }
}
