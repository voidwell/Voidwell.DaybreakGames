using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DBContext
{
    public class PS2DbContext : DbContext
    {
        private readonly DatabaseOptions _options;

        public PS2DbContext (DatabaseOptions options)
        {
            _options = options;
        }

        public DbSet<DbPS2UpdaterScheduler> UpdaterScheduler { get; set; }
        public DbSet<DbCharacter> Characters { get; set; }
        public DbSet<DbOutfitMember> OutfitMembers { get; set; }
        public DbSet<DbCharacterTime> CharacterTimes { get; set; }
        public DbSet<DbOutfit> Outfits { get; set; }
        public DbSet<DbCharacterStat> CharacterStats { get; set; }
        public DbSet<DbCharacterStatByFaction> CharacterStatByFactions { get; set; }
        public DbSet<DbCharacterWeaponStat> CharacterWeaponStats { get; set; }
        public DbSet<DbCharacterWeaponStatByFaction> CharacterWeaponStatByFactions { get; set; }
        public DbSet<DbItem> Items { get; set; }
        public DbSet<DbMapRegion> MapRegions { get; set; }
        public DbSet<DbMapHex> MapHexs { get; set; }
        public DbSet<DbFacilityLink> FacilityLinks { get; set; }
        public DbSet<DbFaction> Factions { get; set; }
        public DbSet<DbProfile> Profiles { get; set; }
        public DbSet<DbTitle> Titles { get; set; }
        public DbSet<DbVehicle> Vehicles { get; set; }
        public DbSet<DbVehicleFaction> VehicleFactions { get; set; }
        public DbSet<DbWorld> Worlds { get; set; }
        public DbSet<DbZone> Zones { get; set; }
        public DbSet<DbAlert> Alerts { get; set; }
        public DbSet<DbPlayerSession> PlayerSessions { get; set; }
        public DbSet<DbMetagameEventCategory> MetagameEventCategories { get; set; }
        public DbSet<DbMetagameEventState> MetagameEventStates { get; set; }
        public DbSet<DbCharacterUpdateQueue> CharacterUpdateQueue { get; set; }
        public DbSet<DbEventAchievementEarned> AchievementEarnedEvents { get; set; }
        public DbSet<DbEventBattlerankUp> BattleRankUpEvents { get; set; }
        public DbSet<DbEventContinentLock> ContinentLockEvents { get; set; }
        public DbSet<DbEventContinentUnlock> ContinentUnlockEvents { get; set; }
        public DbSet<DbEventDeath> EventDeaths { get; set; }
        public DbSet<DbEventFacilityControl> EventFacilityControls { get; set; }
        public DbSet<DbEventGainExperience> GainExperienceEvents { get; set; }
        public DbSet<DbEventMetagameEvent> MetagameEventEvents { get; set; }
        public DbSet<DbEventPlayerFacilityCapture> PlayerFacilityCaptureEvents { get; set; }
        public DbSet<DbEventPlayerFacilityDefend> PlayerFacilityDefendEvents { get; set; }
        public DbSet<DbEventPlayerLogin> PlayerLoginEvents { get; set; }
        public DbSet<DbEventPlayerLogout> PlayerLogoutEvents { get; set; }
        public DbSet<DbEventVehicleDestroy> EventVehicleDestroys { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_options.DBConnectionString, b => b.MigrationsAssembly("Voidwell.DaybreakGames.Data"));

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<DbCharacterStat>()
                .HasKey(a => new { a.CharacterId, a.ProfileId });

            modelBuilder.Entity<DbCharacterStatByFaction>()
                .HasKey(a => new { a.CharacterId, a.ProfileId });

            modelBuilder.Entity<DbCharacterWeaponStat>()
                .HasKey(a => new { a.CharacterId, a.ItemId, a.VehicleId });
            modelBuilder.Entity<DbCharacterWeaponStat>()
                .HasIndex(a => a.Kills);

            modelBuilder.Entity<DbCharacterWeaponStatByFaction>()
                .HasKey(a => new { a.CharacterId, a.ItemId, a.VehicleId });

            modelBuilder.Entity<DbPlayerSession>()
                .HasIndex(a => new { a.CharacterId, a.LoginDate, a.LogoutDate });

            modelBuilder.Entity<DbFacilityLink>()
                .HasKey(a => new { a.ZoneId, a.FacilityIdA, a.FacilityIdB });

            modelBuilder.Entity<DbMapHex>()
                .HasKey(a => new { a.ZoneId, a.MapRegionId });

            modelBuilder.Entity<DbVehicleFaction>()
                .HasKey(a => new { a.VehicleId, a.FactionId });

            modelBuilder.Entity<DbAlert>()
                .HasKey(a => new { a.WorldId, a.MetagameInstanceId });

            modelBuilder.Entity<DbEventAchievementEarned>()
                .HasKey(a => new { a.Timestamp, a.CharacterId });

            modelBuilder.Entity<DbEventBattlerankUp>()
                .HasKey(a => new { a.Timestamp, a.CharacterId });

            modelBuilder.Entity<DbEventContinentLock>()
                .HasKey(a => new { a.Timestamp, a.WorldId, a.ZoneId });

            modelBuilder.Entity<DbEventContinentUnlock>()
                .HasKey(a => new { a.Timestamp, a.WorldId, a.ZoneId });

            modelBuilder.Entity<DbEventDeath>()
                .HasKey(a => new { a.Timestamp, a.AttackerCharacterId, a.CharacterId });

            modelBuilder.Entity<DbEventFacilityControl>()
                .HasKey(a => new { a.Timestamp, a.WorldId, a.FacilityId });

            modelBuilder.Entity<DbEventFacilityControl>()
                .HasKey(a => new { a.Timestamp, a.WorldId, a.FacilityId });

            modelBuilder.Entity<DbEventGainExperience>()
                .HasKey(a => new { a.Timestamp, a.CharacterId, a.ExperienceId });

            modelBuilder.Entity<DbEventMetagameEvent>()
                .HasKey(a => new { a.MetagameId });

            modelBuilder.Entity<DbEventPlayerFacilityCapture>()
                .HasKey(a => new { a.Timestamp, a.CharacterId, a.FacilityId });

            modelBuilder.Entity<DbEventPlayerFacilityDefend>()
                .HasKey(a => new { a.Timestamp, a.CharacterId, a.FacilityId });

            modelBuilder.Entity<DbEventPlayerLogin>()
                .HasKey(a => new { a.Timestamp, a.CharacterId });

            modelBuilder.Entity<DbEventPlayerLogout>()
                .HasKey(a => new { a.Timestamp, a.CharacterId });

            modelBuilder.Entity<DbEventVehicleDestroy>()
                .HasKey(a => new { a.Timestamp, a.AttackerCharacterId, a.CharacterId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
