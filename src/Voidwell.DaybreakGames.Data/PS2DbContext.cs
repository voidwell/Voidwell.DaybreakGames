using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Extensions;
using Voidwell.DaybreakGames.Data.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;

namespace Voidwell.DaybreakGames.Data;

public class PS2DbContext : DbContext
{
    public PS2DbContext(DbContextOptions<PS2DbContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<CharacterLifetimeStat> CharacterLifetimeStats => Set<CharacterLifetimeStat>();
    public DbSet<CharacterLifetimeStatByFaction> CharacterLifetimeStatsByFaction => Set<CharacterLifetimeStatByFaction>();
    public DbSet<CharacterStat> CharacterStats => Set<CharacterStat>();
    public DbSet<CharacterStatByFaction> CharacterStatByFactions => Set<CharacterStatByFaction>();
    public DbSet<CharacterTime> CharacterTimes => Set<CharacterTime>();
    public DbSet<CharacterUpdateQueue> CharacterUpdateQueue => Set<CharacterUpdateQueue>();
    public DbSet<CharacterWeaponStat> CharacterWeaponStats => Set<CharacterWeaponStat>();
    public DbSet<CharacterWeaponStatByFaction> CharacterWeaponStatByFactions => Set<CharacterWeaponStatByFaction>();
    public DbSet<CharacterStatHistory> CharacterStatHistory => Set<CharacterStatHistory>();
    public DbSet<CharacterRating> CharacterRating => Set<CharacterRating>();
    public DbSet<FacilityLink> FacilityLinks => Set<FacilityLink>();
    public DbSet<Faction> Factions => Set<Faction>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemCategory> ItemCategories => Set<ItemCategory>();
    public DbSet<MapHex> MapHexs => Set<MapHex>();
    public DbSet<MapRegion> MapRegions => Set<MapRegion>();
    public DbSet<MetagameEventCategory> MetagameEventCategories => Set<MetagameEventCategory>();
    public DbSet<MetagameEventCategoryZone> MetagameEventCategoryZones => Set<MetagameEventCategoryZone>();
    public DbSet<MetagameEventState> MetagameEventStates => Set<MetagameEventState>();
    public DbSet<Outfit> Outfits => Set<Outfit>();
    public DbSet<OutfitMember> OutfitMembers => Set<OutfitMember>();
    public DbSet<PlayerSession> PlayerSessions => Set<PlayerSession>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Loadout> Loadouts => Set<Loadout>();
    public DbSet<Title> Titles => Set<Title>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleFaction> VehicleFactions => Set<VehicleFaction>();
    public DbSet<World> Worlds => Set<World>();
    public DbSet<Zone> Zones => Set<Zone>();
    public DbSet<SanctionedWeapon> SanctionedWeapons => Set<SanctionedWeapon>();
    public DbSet<ZoneOwnershipSnapshot> ZoneOwnershipSnapshots => Set<ZoneOwnershipSnapshot>();
    public DbSet<DailyWeaponStats> DailyWeaponStats => Set<DailyWeaponStats>();
    public DbSet<Experience> Experience => Set<Experience>();
    public DbSet<WeaponAggregate> WeaponAggregates => Set<WeaponAggregate>();
    public DbSet<DailyPopulation> DailyPopulations => Set<DailyPopulation>();
    public DbSet<Directive> Directives => Set<Directive>();
    public DbSet<DirectiveTier> DirectiveTiers => Set<DirectiveTier>();
    public DbSet<DirectiveTree> DirectiveTrees => Set<DirectiveTree>();
    public DbSet<DirectiveTreeCategory> DirectiveTreeCategories => Set<DirectiveTreeCategory>();
    public DbSet<CharacterDirective> CharacterDirectives => Set<CharacterDirective>();
    public DbSet<CharacterDirectiveObjective> CharacterDirectiveObjectives => Set<CharacterDirectiveObjective>();
    public DbSet<CharacterDirectiveTier> CharacterDirectiveTiers => Set<CharacterDirectiveTier>();
    public DbSet<CharacterDirectiveTree> CharacterDirectiveTrees => Set<CharacterDirectiveTree>();
    public DbSet<Objective> Objectives => Set<Objective>();
    public DbSet<ObjectiveSetToObjective> ObjectiveSetsToObjective => Set<ObjectiveSetToObjective>();
    public DbSet<Reward> Rewards => Set<Reward>();
    public DbSet<RewardGroupToReward> RewardGroupsToReward => Set<RewardGroupToReward>();
    public DbSet<RewardSetToRewardGroup> RewardSetsToRewardGroup => Set<RewardSetToRewardGroup>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<CharacterAchievement> CharacterAchievements => Set<CharacterAchievement>();
    public DbSet<ImageSet> ImageSets => Set<ImageSet>();

    public DbSet<AchievementEarned> AchievementEarnedEvents => Set<AchievementEarned>();
    public DbSet<BattlerankUp> BattleRankUpEvents => Set<BattlerankUp>();
    public DbSet<ContinentLock> ContinentLockEvents => Set<ContinentLock>();
    public DbSet<ContinentUnlock> ContinentUnlockEvents => Set<ContinentUnlock>();
    public DbSet<Death> EventDeaths => Set<Death>();
    public DbSet<FacilityControl> EventFacilityControls => Set<FacilityControl>();
    public DbSet<GainExperience> GainExperienceEvents => Set<GainExperience>();
    public DbSet<MetagameEvent> MetagameEventEvents => Set<MetagameEvent>();
    public DbSet<PlayerFacilityCapture> PlayerFacilityCaptureEvents => Set<PlayerFacilityCapture>();
    public DbSet<PlayerFacilityDefend> PlayerFacilityDefendEvents => Set<PlayerFacilityDefend>();
    public DbSet<PlayerLogin> PlayerLoginEvents => Set<PlayerLogin>();
    public DbSet<PlayerLogout> PlayerLogoutEvents => Set<PlayerLogout>();
    public DbSet<VehicleDestroy> EventVehicleDestroys => Set<VehicleDestroy>();

    public DbSet<UpdaterScheduler> UpdaterScheduler => Set<UpdaterScheduler>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        ApplyConfigurations(builder);
        builder.ConvertToSnakeCaseConvention();
    }

    private static void ApplyConfigurations(ModelBuilder builder)
    {
        var applyGenericMethods = typeof(ModelBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        var applyGenericApplyConfigurationMethods = applyGenericMethods.Where(a => a.IsGenericMethod && a.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase));
        var applyGenericMethod = applyGenericApplyConfigurationMethods.FirstOrDefault(a => a.GetParameters().FirstOrDefault()?.ParameterType.Name == "IEntityTypeConfiguration`1");

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(c => c.IsClass && !c.IsAbstract && !c.ContainsGenericParameters))
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (iface.IsConstructedGenericType && iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                {
                    var applyConcreteMethod = applyGenericMethod!.MakeGenericMethod(iface.GenericTypeArguments[0]);
                    applyConcreteMethod.Invoke(builder, new[] { Activator.CreateInstance(type) });
                    break;
                }
            }
        }
    }
}
