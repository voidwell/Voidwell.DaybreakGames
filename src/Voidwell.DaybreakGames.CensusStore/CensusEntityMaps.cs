using Riok.Mapperly.Abstractions;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore;

/// <summary>
/// Census model -> database entity maps. Every public static method here is discovered by
/// <see cref="CensusToEntityMapper"/> and registered under its (source, destination) type pair.
/// </summary>
// Entities carry navigation properties that census models never populate, so full member coverage is not required.
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public static partial class CensusEntityMaps
{
    [MapProperty(nameof(CensusAchievementModel.AchievementId), nameof(Achievement.Id))]
    [MapProperty("Name.English", nameof(Achievement.Name))]
    [MapProperty("Description.English", nameof(Achievement.Description))]
    public static partial Achievement Map(CensusAchievementModel source);

    [MapProperty(nameof(CensusCharacterAchievementModel.FinishDate), nameof(CharacterAchievement.FinishDate), Use = nameof(NullIfEpoch))]
    public static partial CharacterAchievement Map(CensusCharacterAchievementModel source);

    [MapProperty(nameof(CensusCharacterDirectiveModel.CompletionTimeDate), nameof(CharacterDirective.CompletionTimeDate), Use = nameof(NullIfEpochRequired))]
    public static partial CharacterDirective Map(CensusCharacterDirectiveModel source);

    public static partial CharacterDirectiveObjective Map(CensusCharacterDirectiveObjectiveModel source);

    [MapProperty(nameof(CensusCharacterDirectiveTierModel.CompletionTimeDate), nameof(CharacterDirectiveTier.CompletionTimeDate), Use = nameof(NullIfEpochRequired))]
    public static partial CharacterDirectiveTier Map(CensusCharacterDirectiveTierModel source);

    [MapProperty(nameof(CensusCharacterDirectiveTreeModel.CompletionTimeDate), nameof(CharacterDirectiveTree.CompletionTimeDate), Use = nameof(NullIfEpochRequired))]
    public static partial CharacterDirectiveTree Map(CensusCharacterDirectiveTreeModel source);

    [MapProperty(nameof(CensusDirectiveModel.DirectiveId), nameof(Directive.Id))]
    [MapProperty("Name.English", nameof(Directive.Name))]
    [MapProperty("Description.English", nameof(Directive.Description))]
    public static partial Directive Map(CensusDirectiveModel source);

    [MapProperty("Name.English", nameof(DirectiveTier.Name))]
    public static partial DirectiveTier Map(CensusDirectiveTierModel source);

    [MapProperty(nameof(CensusDirectiveTreeModel.DirectiveTreeId), nameof(DirectiveTree.Id))]
    [MapProperty("Name.English", nameof(DirectiveTree.Name))]
    [MapProperty("Description.English", nameof(DirectiveTree.Description))]
    public static partial DirectiveTree Map(CensusDirectiveTreeModel source);

    [MapProperty(nameof(CensusDirectiveTreeCategoryModel.DirectiveTreeCategoryId), nameof(DirectiveTreeCategory.Id))]
    [MapProperty("Name.English", nameof(DirectiveTreeCategory.Name))]
    public static partial DirectiveTreeCategory Map(CensusDirectiveTreeCategoryModel source);

    [MapProperty(nameof(CensusFactionModel.FactionId), nameof(Faction.Id))]
    [MapProperty("Name.English", nameof(Faction.Name))]
    public static partial Faction Map(CensusFactionModel source);

    [MapProperty(nameof(CensusImageSetModel.ImageSetId), nameof(ImageSet.Id))]
    public static partial ImageSet Map(CensusImageSetModel source);

    [MapProperty(nameof(CensusItemCategoryModel.ItemCategoryId), nameof(ItemCategory.Id))]
    [MapProperty("Name.English", nameof(ItemCategory.Name))]
    public static partial ItemCategory Map(CensusItemCategoryModel source);

    [MapProperty(nameof(CensusItemModel.ItemId), nameof(Item.Id))]
    [MapProperty("Name.English", nameof(Item.Name))]
    [MapProperty("Description.English", nameof(Item.Description))]
    public static partial Item Map(CensusItemModel source);

    [MapProperty(nameof(CensusMapRegionModel.MapRegionId), nameof(MapRegion.Id))]
    [MapProperty(nameof(CensusMapRegionModel.LocationX), nameof(MapRegion.XPos))]
    [MapProperty(nameof(CensusMapRegionModel.LocationY), nameof(MapRegion.YPos))]
    [MapProperty(nameof(CensusMapRegionModel.LocationZ), nameof(MapRegion.ZPos))]
    public static partial MapRegion Map(CensusMapRegionModel source);

    [MapProperty(nameof(CensusMapHexModel.X), nameof(MapHex.XPos))]
    [MapProperty(nameof(CensusMapHexModel.Y), nameof(MapHex.YPos))]
    public static partial MapHex Map(CensusMapHexModel source);

    public static partial FacilityLink Map(CensusFacilityLinkModel source);

    [MapProperty(nameof(CensusMetagameEventCategoryModel.MetagameEventId), nameof(MetagameEventCategory.Id))]
    [MapProperty("Name.English", nameof(MetagameEventCategory.Name))]
    [MapProperty("Description.English", nameof(MetagameEventCategory.Description))]
    public static partial MetagameEventCategory Map(CensusMetagameEventCategoryModel source);

    [MapProperty(nameof(CensusMetagameEventStateModel.MetagameEventStateId), nameof(MetagameEventState.Id))]
    public static partial MetagameEventState Map(CensusMetagameEventStateModel source);

    [MapProperty(nameof(CensusObjectiveModel.ObjectiveId), nameof(Objective.Id))]
    public static partial Objective Map(CensusObjectiveModel source);

    public static partial ObjectiveSetToObjective Map(CensusObjectiveSetToObjectiveModel source);

    [MapProperty(nameof(CensusProfileModel.ProfileId), nameof(Profile.Id))]
    [MapProperty("Name.English", nameof(Profile.Name))]
    public static partial Profile Map(CensusProfileModel source);

    [MapProperty(nameof(CensusRewardModel.RewardId), nameof(Reward.Id))]
    public static partial Reward Map(CensusRewardModel source);

    public static partial RewardSetToRewardGroup Map(CensusRewardSetToRewardGroupModel source);

    public static partial RewardGroupToReward Map(CensusRewardGroupToRewardModel source);

    [MapProperty(nameof(CensusLoadoutModel.LoadoutId), nameof(Loadout.Id))]
    public static partial Loadout Map(CensusLoadoutModel source);

    [MapProperty(nameof(CensusVehicleModel.VehicleId), nameof(Vehicle.Id))]
    [MapProperty("Name.English", nameof(Vehicle.Name))]
    [MapProperty("Description.English", nameof(Vehicle.Description))]
    public static partial Vehicle Map(CensusVehicleModel source);

    public static partial VehicleFaction Map(CensusVehicleFactionModel source);

    [MapProperty(nameof(CensusZoneModel.ZoneId), nameof(Zone.Id))]
    [MapProperty("Name.English", nameof(Zone.Name))]
    [MapProperty("Description.English", nameof(Zone.Description))]
    public static partial Zone Map(CensusZoneModel source);

    [MapProperty(nameof(CensusWorldModel.WorldId), nameof(World.Id))]
    [MapProperty("Name.English", nameof(World.Name))]
    public static partial World Map(CensusWorldModel source);

    [MapProperty(nameof(CensusTitleModel.TitleId), nameof(Title.Id))]
    [MapProperty("Name.English", nameof(Title.Name))]
    public static partial Title Map(CensusTitleModel source);

    // Census reports unset dates as the Unix epoch
    private static DateTime? NullIfEpoch(DateTime? date) => date?.Year == 1970 ? null : date;

    private static DateTime? NullIfEpochRequired(DateTime date) => date.Year == 1970 ? null : date;
}
