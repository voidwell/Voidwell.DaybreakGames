using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class CharacterDetails
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int BattleRank { get; set; }
        public int BattleRankPercentToNext { get; set; }
        public int CertsEarned { get; set; }
        public string Faction { get; set; }
        public int FactionId { get; set; }
        public int? FactionImageId { get; set; }
        public string Title { get; set; }
        public string World { get; set; }
        public int WorldId { get; set; }
        public int PrestigeLevel { get; set; }

        public CharacterDetailsTimes Times { get; set; }
        public CharacterDetailsOutfit Outfit { get; set; }
        public CharacterDetailsLifetimeStats LifetimeStats { get; set; }
        public IEnumerable<CharacterDetailsProfileStat> ProfileStats { get; set; }
        public IEnumerable<CharacterDetailsProfileStatByFaction> ProfileStatsByFaction { get; set; }
        public IEnumerable<CharacterDetailsWeaponStat> WeaponStats { get; set; }
        public IEnumerable<CharacterDetailsVehicleStat> VehicleStats { get; set; }
    }

    public class CharacterDetailsTimes
    {
        public DateTime CreatedDate { get; set; }
        public DateTime LastSaveDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int MinutesPlayed { get; set; }
    }

    public class CharacterDetailsOutfit
    {
        public string Id { get; set; }
        public DateTime MemberSinceDate { get; set; }
        public string Rank { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public DateTime CreatedDate { get; set; }
        public int MemberCount { get; set; }
    }

    public class CharacterDetailsLifetimeStats
    {
        public int AchievementCount { get; set; }
        public int AssistCount { get; set; }
        public int FacilityDefendedCount { get; set; }
        public int MedalCount { get; set; }
        public int SkillPoints { get; set; }
        public int Deaths { get; set; }
        public int FireCount { get; set; }
        public int HitCount { get; set; }
        public int PlayTime { get; set; }
        public int Score { get; set; }
        public int DominationCount { get; set; }
        public int FacilityCaptureCount { get; set; }
        public int RevengeCount { get; set; }
        public int DamageGiven { get; set; }
        public int DamageTakenBy { get; set; }
        public int Headshots { get; set; }
        public int KilledBy { get; set; }
        public int Kills { get; set; }
        public int VehicleKills { get; set; }
    }

    public class CharacterDetailsProfileStat
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set;}
        public int? ImageId { get; set; }
        public int Deaths { get; set; }
        public int FireCount { get; set; }
        public int HitCount { get; set; }
        public int PlayTime { get; set; }
        public int Score { get; set; }
        public int KilledBy { get; set; }
        public int Kills { get; set; }
    }

    public class CharacterDetailsProfileStatByFaction
    {
        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public int? ImageId { get; set; }
        public CharacterDetailsProfileStatByFactionValue KilledBy { get; set; }
        public CharacterDetailsProfileStatByFactionValue Kills { get; set; }
    }

    public class CharacterDetailsProfileStatByFactionValue
    {
        public int Vs { get; set; }
        public int Nc { get; set; }
        public int Tr { get; set; }
    }

    public class CharacterDetailsWeaponStat
    {
        public int ItemId { get; set; }
        public string Category { get; set; }
        public int? ImageId { get; set; }
        public string Name { get; set; }
        public int? VehicleId { get; set; }
        public string VehicleName { get; set; }
        public int? VehicleImageId { get; set; }
        public CharacterDetailsWeaponStatValue Stats { get; set; }
    }

    public class CharacterDetailsWeaponStatValue
    {
        public int? Deaths { get; set; }
        public int? FireCount { get; set; }
        public int? HitCount { get; set; }
        public int? PlayTime { get; set; }
        public int? DamageGiven { get; set; }
        public int? Headshots { get; set; }
        public int? Kills { get; set; }
        public int? VehicleKills { get; set; }
        public int? Score { get; set; }
        public int? DamageTakenBy { get; set; }
        public int? KilledBy { get; set; }
        public double? KillDeathRatio { get; set; }
        public double? Accuracy { get; set; }
        public double? HeadshotRatio { get; set; }
        public double? KillsPerHour { get; set; }
        public double? VehicleKillsPerHour { get; set; }
        public double? ScorePerMinute { get; set; }
        public double? LandedPerKill { get; set; }
        public double? ShotsPerKill { get; set; }
        public double? KillDeathRatioDelta { get; set; }
        public double? AccuracyDelta { get; set; }
        public double? HsrDelta { get; set; }
        public double? KphDelta { get; set; }
        public double? VehicleKphDelta { get; set; }
    }

    public class CharacterDetailsVehicleStat
    {
        public int VehicleId { get; set; }
        public int? DamageTakenBy { get; set; }
        public int? KilledBy { get; set; }
        public int? Score { get; set; }
        public int? Deaths { get; set; }
        public int? FireCount { get; set; }
        public int? HitCount { get; set; }
        public int? PlayTime { get; set; }
        public int? DamageGiven { get; set; }
        public int? Headshots { get; set; }
        public int? Kills { get; set; }
        public int? VehicleKills { get; set; }
        public int? PilotKills { get; set; }
        public int? PilotDeaths { get; set; }
        public int? PilotVehicleKills { get; set; }
        public int? PilotPlayTime { get; set; }
        public int? PilotDamageGiven { get; set; }
        public int? PilotDamageTakenBy { get; set; }
        public int? PilotScore { get; set; }
        public int? PilotKilledBy { get; set; }
        public int? PilotFireCount { get; set; }
        public int? PilotHitCount { get; set; }
        public int? PilotHeadshots { get; set; }
    }
}
