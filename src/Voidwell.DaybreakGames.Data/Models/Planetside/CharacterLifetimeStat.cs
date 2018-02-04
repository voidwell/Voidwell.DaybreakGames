using System.ComponentModel.DataAnnotations;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    public class CharacterLifetimeStat
    {
        [Required]
        public string CharacterId { get; set; }

        public int? AchievementCount { get; set; }
        public int? AssistCount { get; set; }
        public int? FacilityDefendedCount { get; set; }
        public int? MedalCount { get; set; }
        public int? SkillPoints { get; set; }
        public int? WeaponDeaths { get; set; }
        public int? WeaponFireCount { get; set; }
        public int? WeaponHitCount { get; set; }
        public int? WeaponPlayTime { get; set; }
        public int? WeaponScore { get; set; }

        // From By Faction
        public int? DominationCount { get; set; }
        public int? FacilityCaptureCount { get; set; }
        public int? RevengeCount { get; set; }
        public int? WeaponDamageGiven { get; set; }
        public int? WeaponDamageTakenBy { get; set; }
        public int? WeaponHeadshots { get; set; }
        public int? WeaponKills { get; set; }
        public int? WeaponVehicleKills { get; set; }

        public Character Character { get; set; }
    }
}
