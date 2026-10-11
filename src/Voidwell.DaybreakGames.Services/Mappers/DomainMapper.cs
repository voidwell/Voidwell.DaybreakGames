using System.Text.Json;
using Riok.Mapperly.Abstractions;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Mappers;

// Domain models are projections of the data models and intentionally leave many members unset.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public static partial class DomainMapper
{
    // Census -> Domain

    [MapProperty(nameof(CensusCharacterModel.CharacterId), nameof(CharacterSearchResult.Id))]
    [MapProperty("Name.First", nameof(CharacterSearchResult.Name))]
    [MapProperty("BattleRank.Value", nameof(CharacterSearchResult.BattleRank))]
    [MapProperty("Times.LastLoginDate", nameof(CharacterSearchResult.LastLogin))]
    public static partial CharacterSearchResult MapSearchResult(CensusCharacterModel source);

    // Domain -> Domain

    [MapProperty("Times.LastSaveDate", nameof(SimpleCharacterDetails.LastSaved))]
    [MapProperty(nameof(CharacterDetails.Faction), nameof(SimpleCharacterDetails.FactionName))]
    [MapProperty("Outfit.Alias", nameof(SimpleCharacterDetails.OutfitAlias))]
    [MapProperty("Outfit.Name", nameof(SimpleCharacterDetails.OutfitName))]
    [MapProperty("LifetimeStats.Kills", nameof(SimpleCharacterDetails.Kills))]
    [MapProperty("LifetimeStats.Deaths", nameof(SimpleCharacterDetails.Deaths))]
    [MapProperty("LifetimeStats.Score", nameof(SimpleCharacterDetails.Score))]
    [MapProperty("LifetimeStats.PlayTime", nameof(SimpleCharacterDetails.PlayTime))]
    [MapProperty("Times.MinutesPlayed", nameof(SimpleCharacterDetails.TotalPlayTimeMinutes))]
    [MapProperty("InfantryStats.IVIScore", nameof(SimpleCharacterDetails.IVIScore))]
    [MapProperty("InfantryStats.KillDeathRatio", nameof(SimpleCharacterDetails.IVIKillDeathRatio))]
    [MapProperty(nameof(CharacterDetails.PrestigeLevel), nameof(SimpleCharacterDetails.Prestige))]
    [MapPropertyFromSource(nameof(SimpleCharacterDetails.PlayTimeInMax), Use = nameof(GetPlayTimeInMax))]
    [MapperIgnoreTarget(nameof(SimpleCharacterDetails.KillDeathRatio))]
    [MapperIgnoreTarget(nameof(SimpleCharacterDetails.HeadshotRatio))]
    [MapperIgnoreTarget(nameof(SimpleCharacterDetails.KillsPerHour))]
    [MapperIgnoreTarget(nameof(SimpleCharacterDetails.TotalKillsPerHour))]
    [MapperIgnoreTarget(nameof(SimpleCharacterDetails.SiegeLevel))]
    [MapperIgnoreTarget(nameof(SimpleCharacterDetails.MostPlayedWeaponName))]
    [MapperIgnoreTarget(nameof(SimpleCharacterDetails.MostPlayedWeaponId))]
    [MapperIgnoreTarget(nameof(SimpleCharacterDetails.MostPlayedWeaponKills))]
    [MapperIgnoreTarget(nameof(SimpleCharacterDetails.MostPlayedClassName))]
    [MapperIgnoreTarget(nameof(SimpleCharacterDetails.MostPlayedClassId))]
    public static partial SimpleCharacterDetails MapSimpleDetails(CharacterDetails source);

    // Data -> Domain

    [MapProperty("Faction.Name", nameof(CharacterDetails.Faction))]
    [MapProperty("Faction.ImageId", nameof(CharacterDetails.FactionImageId))]
    [MapProperty("Title.Name", nameof(CharacterDetails.Title))]
    [MapProperty("World.Name", nameof(CharacterDetails.World))]
    [MapProperty(nameof(Character.Time), nameof(CharacterDetails.Times))]
    [MapProperty(nameof(Character.Stats), nameof(CharacterDetails.ProfileStats), Use = nameof(MapProfileStats))]
    [MapProperty(nameof(Character.StatsByFaction), nameof(CharacterDetails.ProfileStatsByFaction), Use = nameof(MapProfileStatsByFaction))]
    [MapProperty(nameof(Character.WeaponStats), nameof(CharacterDetails.WeaponStats), Use = nameof(MapWeaponStats))]
    [MapProperty(nameof(Character.StatsHistory), nameof(CharacterDetails.StatsHistory), Use = nameof(MapStatsHistories))]
    [MapProperty(nameof(Character.OutfitMembership), nameof(CharacterDetails.Outfit))]
    [MapPropertyFromSource(nameof(CharacterDetails.VehicleStats), Use = nameof(GetVehicleStats))]
    [MapperIgnoreTarget(nameof(CharacterDetails.InfantryStats))]
    public static partial CharacterDetails MapCharacterDetails(Character source);

    public static partial CharacterDetailsTimes MapTimes(CharacterTime source);

    [MapProperty(nameof(CharacterLifetimeStat.WeaponKills), nameof(CharacterDetailsLifetimeStats.Kills))]
    [MapProperty(nameof(CharacterLifetimeStat.WeaponPlayTime), nameof(CharacterDetailsLifetimeStats.PlayTime))]
    [MapProperty(nameof(CharacterLifetimeStat.WeaponVehicleKills), nameof(CharacterDetailsLifetimeStats.VehicleKills))]
    [MapProperty(nameof(CharacterLifetimeStat.WeaponDamageGiven), nameof(CharacterDetailsLifetimeStats.DamageGiven))]
    [MapProperty(nameof(CharacterLifetimeStat.WeaponDamageTakenBy), nameof(CharacterDetailsLifetimeStats.DamageTakenBy))]
    [MapProperty(nameof(CharacterLifetimeStat.WeaponFireCount), nameof(CharacterDetailsLifetimeStats.FireCount))]
    [MapProperty(nameof(CharacterLifetimeStat.WeaponHitCount), nameof(CharacterDetailsLifetimeStats.HitCount))]
    [MapProperty(nameof(CharacterLifetimeStat.WeaponScore), nameof(CharacterDetailsLifetimeStats.Score))]
    [MapProperty(nameof(CharacterLifetimeStat.WeaponDeaths), nameof(CharacterDetailsLifetimeStats.Deaths))]
    [MapProperty(nameof(CharacterLifetimeStat.WeaponHeadshots), nameof(CharacterDetailsLifetimeStats.Headshots))]
    public static partial CharacterDetailsLifetimeStats MapLifetimeStats(CharacterLifetimeStat source);

    [MapProperty("Profile.Name", nameof(CharacterDetailsProfileStat.ProfileName))]
    [MapProperty("Profile.ImageId", nameof(CharacterDetailsProfileStat.ImageId))]
    public static partial CharacterDetailsProfileStat MapProfileStat(CharacterStat source);

    [MapProperty("Profile.Name", nameof(CharacterDetailsProfileStatByFaction.ProfileName))]
    [MapProperty("Profile.ImageId", nameof(CharacterDetailsProfileStatByFaction.ImageId))]
    [MapPropertyFromSource(nameof(CharacterDetailsProfileStatByFaction.Kills), Use = nameof(GetKillsByFaction))]
    [MapPropertyFromSource(nameof(CharacterDetailsProfileStatByFaction.KilledBy), Use = nameof(GetKilledByFaction))]
    public static partial CharacterDetailsProfileStatByFaction MapProfileStatByFaction(CharacterStatByFaction source);

    [MapProperty(nameof(CharacterStatHistory.Day), nameof(CharacterDetailsStatsHistory.Day), Use = nameof(DeserializeValues))]
    [MapProperty(nameof(CharacterStatHistory.Week), nameof(CharacterDetailsStatsHistory.Week), Use = nameof(DeserializeValues))]
    [MapProperty(nameof(CharacterStatHistory.Month), nameof(CharacterDetailsStatsHistory.Month), Use = nameof(DeserializeValues))]
    public static partial CharacterDetailsStatsHistory MapStatsHistory(CharacterStatHistory source);

    [MapProperty(nameof(OutfitMember.OutfitId), nameof(CharacterDetailsOutfit.Id))]
    [MapProperty("Outfit.Name", nameof(CharacterDetailsOutfit.Name))]
    [MapProperty("Outfit.Alias", nameof(CharacterDetailsOutfit.Alias))]
    [MapProperty("Outfit.CreatedDate", nameof(CharacterDetailsOutfit.CreatedDate))]
    [MapProperty("Outfit.MemberCount", nameof(CharacterDetailsOutfit.MemberCount))]
    public static partial CharacterDetailsOutfit MapOutfit(OutfitMember source);

    [MapProperty("Item.Name", nameof(CharacterDetailsWeaponStat.Name))]
    [MapProperty("Item.ItemCategory.Name", nameof(CharacterDetailsWeaponStat.Category))]
    [MapProperty("Item.ImageId", nameof(CharacterDetailsWeaponStat.ImageId))]
    [MapProperty("Vehicle.Name", nameof(CharacterDetailsWeaponStat.VehicleName))]
    [MapProperty("Vehicle.ImageId", nameof(CharacterDetailsWeaponStat.VehicleImageId))]
    [MapPropertyFromSource(nameof(CharacterDetailsWeaponStat.Stats), Use = nameof(MapWeaponStatValue))]
    public static partial CharacterDetailsWeaponStat MapWeaponStat(CharacterWeaponStat source);

    public static partial CharacterDetailsWeaponStatValue MapWeaponStatValue(CharacterWeaponStat source);

    // Directives

    [MapperIgnoreTarget(nameof(CharacterDirectivesOutlineCategory.Trees))]
    public static partial CharacterDirectivesOutlineCategory MapOutlineCategory(DirectiveTreeCategory source);

    [MapperIgnoreTarget(nameof(CharacterDirectivesOutlineTree.Tiers))]
    public static partial CharacterDirectivesOutlineTree MapOutlineTree(DirectiveTree source);

    [MapProperty(nameof(DirectiveTier.DirectiveTierId), nameof(CharacterDirectivesOutlineTier.TierId))]
    [MapProperty(nameof(DirectiveTier.DirectiveTreeId), nameof(CharacterDirectivesOutlineTier.TreeId))]
    [MapperIgnoreTarget(nameof(CharacterDirectivesOutlineTier.Directives))]
    public static partial CharacterDirectivesOutlineTier MapOutlineTier(DirectiveTier source);

    [MapProperty(nameof(RewardGroupToReward.RewardId), nameof(DirectivesOutlineReward.Id))]
    [MapProperty("Reward.Item.Name", nameof(DirectivesOutlineReward.Name))]
    [MapProperty("Reward.Item.ImageId", nameof(DirectivesOutlineReward.ImageId))]
    public static partial DirectivesOutlineReward MapOutlineReward(RewardGroupToReward source);

    public static partial List<DirectivesOutlineReward> MapOutlineRewards(IEnumerable<RewardGroupToReward> source);

    [MapProperty(nameof(Directive.DirectiveTierId), nameof(CharacterDirectivesOutlineDirective.TierId))]
    [MapProperty(nameof(Directive.DirectiveTreeId), nameof(CharacterDirectivesOutlineDirective.TreeId))]
    [MapPropertyFromSource(nameof(CharacterDirectivesOutlineDirective.ImageId), Use = nameof(GetDirectiveImageId))]
    [MapPropertyFromSource(nameof(CharacterDirectivesOutlineDirective.Objective), Use = nameof(GetFirstObjective))]
    [MapperIgnoreTarget(nameof(CharacterDirectivesOutlineDirective.CompletionDate))]
    [MapperIgnoreTarget(nameof(CharacterDirectivesOutlineDirective.Progress))]
    public static partial CharacterDirectivesOutlineDirective MapOutlineDirective(Directive source);

    [MapProperty(nameof(Objective.ObjectiveTypeId), nameof(DirectivesOutlineObjective.TypeId))]
    [MapperIgnoreTarget(nameof(DirectivesOutlineObjective.GoalValue))]
    public static partial DirectivesOutlineObjective MapOutlineObjective(Objective source);

    // Map

    public static partial ZoneLink MapZoneLink(FacilityLink source);

    public static partial List<ZoneLink> MapZoneLinks(IEnumerable<FacilityLink> source);

    [MapProperty(nameof(MapHex.XPos), nameof(ZoneHex.X))]
    [MapProperty(nameof(MapHex.YPos), nameof(ZoneHex.Y))]
    public static partial ZoneHex MapZoneHex(MapHex source);

    public static partial List<ZoneHex> MapZoneHexes(IEnumerable<MapHex> source);

    [MapProperty(nameof(MapRegion.Id), nameof(ZoneRegion.RegionId))]
    [MapProperty(nameof(MapRegion.XPos), nameof(ZoneRegion.X))]
    [MapProperty(nameof(MapRegion.YPos), nameof(ZoneRegion.Y))]
    [MapProperty(nameof(MapRegion.ZPos), nameof(ZoneRegion.Z))]
    public static partial ZoneRegion MapZoneRegion(MapRegion source);

    public static partial List<ZoneRegion> MapZoneRegions(IEnumerable<MapRegion> source);

    // Character collections are materialized (and null becomes empty) because callers enumerate and mutate the mapped items.
    private static IEnumerable<CharacterDetailsProfileStat> MapProfileStats(IEnumerable<CharacterStat>? source)
    {
        return source?.Select(MapProfileStat).ToList() ?? [];
    }

    private static IEnumerable<CharacterDetailsProfileStatByFaction> MapProfileStatsByFaction(IEnumerable<CharacterStatByFaction>? source)
    {
        return source?.Select(MapProfileStatByFaction).ToList() ?? [];
    }

    private static IEnumerable<CharacterDetailsWeaponStat> MapWeaponStats(IEnumerable<CharacterWeaponStat>? source)
    {
        return source?.Select(MapWeaponStat).ToList() ?? [];
    }

    private static IEnumerable<CharacterDetailsStatsHistory> MapStatsHistories(IEnumerable<CharacterStatHistory>? source)
    {
        return source?.Select(MapStatsHistory).ToList() ?? [];
    }

    private static int GetPlayTimeInMax(CharacterDetails source)
    {
        // Profile 7 is the MAX class
        return source.ProfileStats?.FirstOrDefault(a => a.ProfileId == 7)?.PlayTime ?? 0;
    }

    private static IEnumerable<int>? DeserializeValues(string? json)
    {
        return json == null ? null : JsonSerializer.Deserialize<IEnumerable<int>>(json, (JsonSerializerOptions?)null);
    }

    private static CharacterDetailsProfileStatByFactionValue GetKillsByFaction(CharacterStatByFaction source)
    {
        return new CharacterDetailsProfileStatByFactionValue
        {
            Vs = source.KillsVS.GetValueOrDefault(),
            Nc = source.KillsNC.GetValueOrDefault(),
            Tr = source.KillsTR.GetValueOrDefault()
        };
    }

    private static CharacterDetailsProfileStatByFactionValue GetKilledByFaction(CharacterStatByFaction source)
    {
        return new CharacterDetailsProfileStatByFactionValue
        {
            Vs = source.KilledByVS.GetValueOrDefault(),
            Nc = source.KilledByNC.GetValueOrDefault(),
            Tr = source.KilledByTR.GetValueOrDefault()
        };
    }

    private static int? GetDirectiveImageId(Directive source)
    {
        return source.ImageSet != null ? source.ImageSet.ImageId : source.ImageId;
    }

    private static DirectivesOutlineObjective GetFirstObjective(Directive source)
    {
        var objective = source.ObjectiveSet?.Objectives?.FirstOrDefault();

        return objective != null ? MapOutlineObjective(objective) : new DirectivesOutlineObjective();
    }

    private static IEnumerable<CharacterDetailsVehicleStat> GetVehicleStats(Character source)
    {
        var weaponStats = source.WeaponStats!;
        var vehicleStats = CalculateVehicleStats(weaponStats).ToList();

        weaponStats.Where(a => a.VehicleId != 0 && a.ItemId == 0).GroupBy(a => a.VehicleId).ToList().ForEach(item =>
        {
            var stats = item.FirstOrDefault();

            var stat = vehicleStats.FirstOrDefault(a => a.VehicleId == item.Key);
            if (stat != null)
            {
                stat.PilotKills = stats!.Kills.GetValueOrDefault();
                stat.PilotPlayTime = stats.PlayTime.GetValueOrDefault();
                stat.PilotVehicleKills = stats.VehicleKills.GetValueOrDefault();
            }
        });

        return vehicleStats;
    }

    private static IEnumerable<CharacterDetailsVehicleStat> CalculateVehicleStats(IEnumerable<CharacterWeaponStat> weaponStats)
    {
        foreach (var s in weaponStats.Where(a => a.VehicleId != 0).GroupBy(a => a.VehicleId))
        {
            var vehicleWeaponStats = s.Where(a => a.ItemId != 0);
            var vehicleStats = s.FirstOrDefault(a => a.ItemId == 0);

            yield return new CharacterDetailsVehicleStat
            {
                // Gunner stats
                VehicleId = s.Key,
                DamageGiven = vehicleWeaponStats.Sum(a => a.DamageGiven.GetValueOrDefault()),
                DamageTakenBy = vehicleWeaponStats.Sum(a => a.DamageTakenBy.GetValueOrDefault()),
                Deaths = vehicleWeaponStats.Sum(a => a.Deaths.GetValueOrDefault()),
                FireCount = vehicleWeaponStats.Sum(a => a.FireCount.GetValueOrDefault()),
                HitCount = vehicleWeaponStats.Sum(a => a.HitCount.GetValueOrDefault()),
                VehicleKills = vehicleWeaponStats.Sum(a => a.VehicleKills.GetValueOrDefault()),
                Headshots = vehicleWeaponStats.Sum(a => a.Headshots.GetValueOrDefault()),
                KilledBy = vehicleWeaponStats.Sum(a => a.KilledBy.GetValueOrDefault()),
                Kills = vehicleWeaponStats.Sum(a => a.Kills.GetValueOrDefault()),
                PlayTime = vehicleWeaponStats.Sum(a => a.PlayTime.GetValueOrDefault()),
                Score = vehicleWeaponStats.Sum(a => a.Score.GetValueOrDefault()),

                // Pilot stats
                PilotDamageGiven = vehicleStats?.DamageGiven.GetValueOrDefault(),
                PilotDamageTakenBy = vehicleStats?.DamageTakenBy.GetValueOrDefault(),
                PilotDeaths = vehicleStats?.Deaths.GetValueOrDefault(),
                PilotFireCount = vehicleStats?.FireCount.GetValueOrDefault(),
                PilotHitCount = vehicleStats?.HitCount.GetValueOrDefault(),
                PilotVehicleKills = vehicleStats?.VehicleKills.GetValueOrDefault(),
                PilotHeadshots = vehicleStats?.Headshots.GetValueOrDefault(),
                PilotKilledBy = vehicleStats?.KilledBy.GetValueOrDefault(),
                PilotKills = vehicleStats?.Kills.GetValueOrDefault(),
                PilotPlayTime = vehicleStats?.PlayTime.GetValueOrDefault(),
                PilotScore = vehicleStats?.Score.GetValueOrDefault()
            };
        }
    }
}

/// <summary>
/// Applies a character's progress onto an existing directive outline. Only the members the
/// character data supplies are written; everything else on the outline is left untouched.
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public static partial class CharacterDirectiveProgressMapper
{
    [MapProperty(nameof(CharacterDirectiveTree.CompletionTimeDate), nameof(CharacterDirectivesOutlineTree.CompletionDate))]
    public static partial void Apply(CharacterDirectiveTree source, CharacterDirectivesOutlineTree target);

    [MapProperty(nameof(CharacterDirectiveTier.CompletionTimeDate), nameof(CharacterDirectivesOutlineTier.CompletionDate))]
    public static partial void Apply(CharacterDirectiveTier source, CharacterDirectivesOutlineTier target);

    [MapProperty(nameof(CharacterDirective.CompletionTimeDate), nameof(CharacterDirectivesOutlineDirective.CompletionDate))]
    public static partial void Apply(CharacterDirective source, CharacterDirectivesOutlineDirective target);
}
