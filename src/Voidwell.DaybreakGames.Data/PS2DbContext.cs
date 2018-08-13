using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Voidwell.DaybreakGames.Data.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;

namespace Voidwell.DaybreakGames.Data
{
    public class PS2DbContext : DbContext
    {
        public PS2DbContext(DbContextOptions<PS2DbContext> options)
            : base(options)
        {
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
        public DbSet<Title> Titles { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleFaction> VehicleFactions { get; set; }
        public DbSet<World> Worlds { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<SanctionedWeapon> SanctionedWeapons { get; set; }
        public DbSet<ZoneOwnershipSnapshot> ZoneOwnershipSnapshots { get; set; }
        public DbSet<DailyWeaponStats> DailyWeaponStats { get; set; }
        public DbSet<Experience> Experience { get; set; }

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
            ConvertToConvention(builder);
        }

        private static void ApplyConfigurations(ModelBuilder builder)
        {
            var applyGenericMethods = typeof(ModelBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var applyGenericApplyConfigurationMethods = applyGenericMethods.Where(a => a.IsGenericMethod && a.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase));
            var applyGenericMethod = applyGenericApplyConfigurationMethods.Where(a => a.GetParameters().FirstOrDefault().ParameterType.Name == "IEntityTypeConfiguration`1").FirstOrDefault();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(c => c.IsClass && !c.IsAbstract && !c.ContainsGenericParameters))
            {
                foreach (var iface in type.GetInterfaces())
                {
                    if (iface.IsConstructedGenericType && iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {
                        var applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                        applyConcreteMethod.Invoke(builder, new object[] { Activator.CreateInstance(type) });
                        break;
                    }
                }
            }
        }

        private static void ConvertToConvention(ModelBuilder builder)
        {
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.Relational().TableName = ToSnakeCase(entity.Relational().TableName);

                // Replace column names
                foreach (var property in entity.GetProperties())
                {
                    property.Relational().ColumnName = ToSnakeCase(property.Name);
                }

                foreach (var key in entity.GetKeys())
                {
                    key.Relational().Name = ToSnakeCase(key.Relational().Name);
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.Relational().Name = ToSnakeCase(key.Relational().Name);
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.Relational().Name = ToSnakeCase(index.Relational().Name);
                }
            }
        }

        private static string ToSnakeCase(string input)
        {
            var result = Regex.Replace(input, ".[A-Z]", m => m.Value[0] + "_" + m.Value[1]);

            return result.ToLower();
        }
    }
}
