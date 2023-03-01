using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using Voidwell.DaybreakGames.Data.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.Microservice.EntityFramework;

namespace Voidwell.DaybreakGames.Data
{
    public class PS2DbContext : DbContext
    {
        public PS2DbContext(DbContextOptions<PS2DbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<CharacterLifetimeStat> CharacterLifetimeStats { get; set; }
        public DbSet<CharacterLifetimeStatByFaction> CharacterLifetimeStatsByFaction { get; set; }
        public DbSet<CharacterStat> CharacterStats { get; set; }
        public DbSet<CharacterStatByFaction> CharacterStatByFactions { get; set; }
        public DbSet<CharacterTime> CharacterTimes { get; set; }
        public DbSet<CharacterUpdateQueue> CharacterUpdateQueue { get; set; }
        public DbSet<CharacterWeaponStat> CharacterWeaponStats { get; set; }
        public DbSet<CharacterWeaponStatByFaction> CharacterWeaponStatByFactions { get; set; }
        public DbSet<CharacterStatHistory> CharacterStatHistory { get; set; }
        public DbSet<CharacterRating> CharacterRating { get; set; }
        public DbSet<FacilityLink> FacilityLinks { get; set; }
        public DbSet<Faction> Factions { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<MapHex> MapHexs { get; set; }
        public DbSet<MapRegion> MapRegions { get; set; }
        public DbSet<MetagameEventCategory> MetagameEventCategories { get; set; }
        public DbSet<MetagameEventCategoryZone> MetagameEventCategoryZones { get; set; }
        public DbSet<MetagameEventState> MetagameEventStates { get; set; }
        public DbSet<Outfit> Outfits { get; set; }
        public DbSet<OutfitMember> OutfitMembers { get; set; }
        public DbSet<PlayerSession> PlayerSessions { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Loadout> Loadouts { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleFaction> VehicleFactions { get; set; }
        public DbSet<World> Worlds { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<SanctionedWeapon> SanctionedWeapons { get; set; }
        public DbSet<ZoneOwnershipSnapshot> ZoneOwnershipSnapshots { get; set; }
        public DbSet<DailyWeaponStats> DailyWeaponStats { get; set; }
        public DbSet<Experience> Experience { get; set; }
        public DbSet<WeaponAggregate> WeaponAggregates { get; set; }
        public DbSet<DailyPopulation> DailyPopulations { get; set; }
        public DbSet<Directive> Directives { get; set; }
        public DbSet<DirectiveTier> DirectiveTiers { get; set; }
        public DbSet<DirectiveTree> DirectiveTrees { get; set; }
        public DbSet<DirectiveTreeCategory> DirectiveTreeCategories { get; set; }
        public DbSet<CharacterDirective> CharacterDirectives { get; set; }
        public DbSet<CharacterDirectiveObjective> CharacterDirectiveObjectives { get; set; }
        public DbSet<CharacterDirectiveTier> CharacterDirectiveTiers { get; set; }
        public DbSet<CharacterDirectiveTree> CharacterDirectiveTrees { get; set; }
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<ObjectiveSetToObjective> ObjectiveSetsToObjective { get; set; }
        public DbSet<Reward> Rewards { get; set; }
        public DbSet<RewardGroupToReward> RewardGroupsToReward { get; set; }
        public DbSet<RewardSetToRewardGroup> RewardSetsToRewardGroup { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<CharacterAchievement> CharacterAchievements { get; set; }

        public DbSet<AchievementEarned> AchievementEarnedEvents { get; set; }
        public DbSet<BattlerankUp> BattleRankUpEvents { get; set; }
        public DbSet<ContinentLock> ContinentLockEvents { get; set; }
        public DbSet<ContinentUnlock> ContinentUnlockEvents { get; set; }
        public DbSet<Death> EventDeaths { get; set; }
        public DbSet<FacilityControl> EventFacilityControls { get; set; }
        public DbSet<GainExperience> GainExperienceEvents { get; set; }
        public DbSet<MetagameEvent> MetagameEventEvents { get; set; }
        public DbSet<PlayerFacilityCapture> PlayerFacilityCaptureEvents { get; set; }
        public DbSet<PlayerFacilityDefend> PlayerFacilityDefendEvents { get; set; }
        public DbSet<PlayerLogin> PlayerLoginEvents { get; set; }
        public DbSet<PlayerLogout> PlayerLogoutEvents { get; set; }
        public DbSet<VehicleDestroy> EventVehicleDestroys { get; set; }

        public DbSet<UpdaterScheduler> UpdaterScheduler { get; set; }

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
                        var applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                        applyConcreteMethod.Invoke(builder, new[] { Activator.CreateInstance(type) });
                        break;
                    }
                }
            }
        }
    }
}
