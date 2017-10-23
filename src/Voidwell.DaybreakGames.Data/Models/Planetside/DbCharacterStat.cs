using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("CharacterStat")]
    public class DbCharacterStat
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string ProfileId { get; set; }

        public int AchievementCount { get; set; }
        public int AssistCount { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int FacilityDefendedCount { get; set; }
        public int FireCount { get; set; }
        public int HitCount { get; set; }
        public int MedalCount { get; set; }
        public int PlayTime { get; set; }
        public int Score { get; set; }
        public int SkillPoints { get; set; }
        public int RevengeCount { get; set; }
        public int KilledBy { get; set; }
        public int FacilityCaptureCount { get; set; }
        public int DominationCount { get; set; }
        public int WeaponDeaths { get; set; }
        public int WeaponFireCount { get; set; }
        public int WeaponHitCount { get; set; }
        public int WeaponPlayTime { get; set; }
        public int WeaponScore { get; set; }
        public int WeaponVehicleKills { get; set; }
        public int WeaponKills { get; set; }
        public int WeaponKilledBy { get; set; }
        public int WeaponHeadshots { get; set; }
        public int WeaponDamageTakenBy { get; set; }
        public int WeaponDamageGiven { get; set; }

        [ForeignKey("ProfileId")]
        public DbProfile Profile { get; set; }
        [ForeignKey("CharacterId")]
        public DbCharacter Character { get; set; }
    }
}
