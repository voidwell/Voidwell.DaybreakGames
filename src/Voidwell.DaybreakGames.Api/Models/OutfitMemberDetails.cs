using System;

namespace Voidwell.DaybreakGames.Api.Models
{
    public class OutfitMemberDetails
    {
        public string CharacterId { get; set; }
        public string Name { get; set; }
        public DateTime MemberSinceDate { get; set; }
        public int RankOrdinal { get; set; }
        public string Rank { get; set; }
        public int? BattleRank { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public OutfitMemberDetailsStats LifetimeStats { get; set; }
    }

    public class OutfitMemberDetailsStats
    {
        public int? WeaponKills { get; set; }
        public int? WeaponDeaths { get; set; }
        public int? WeaponHeadshots { get; set; }
        public int? FacilityDefendedCount { get; set; }
        public int? FacilityCaptureCount { get; set; }
        public int? AssistCount { get; set; }
        public int? WeaponHitCount { get; set; }
        public int? WeaponFireCount { get; set; }
        public int? WeaponScore { get; set; }
        public int? WeaponPlayTime { get; set; }
        public int? WeaponVehicleKills { get; set; }
        public int? DominationCount { get; set; }
        public int? RevengeCount { get; set; }
    }
}
