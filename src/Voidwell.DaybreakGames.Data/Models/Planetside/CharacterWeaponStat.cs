using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterWeaponStat
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public int ItemId { get; set; }
        public int? VehicleId { get; set; }

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

        public Item Item { get; set; }
        public Vehicle Vehicle { get; set; }

        public Character Character { get; set; }
    }
}
