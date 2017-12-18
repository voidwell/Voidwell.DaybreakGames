using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data
{
    public class PS2DbContext : DbContext
    {
        public PS2DbContext(DbContextOptions<PS2DbContext> options)
            : base(options)
        {
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
        public DbSet<DbItemCategory> ItemCategories { get; set; }
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<DbCharacterStat>()
                .HasKey(a => new { a.CharacterId, a.ProfileId });
            builder.Entity<DbCharacterStat>().Property(a => a.AchievementCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.AssistCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.Deaths).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.DominationCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.FacilityCaptureCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.FacilityDefendedCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.FireCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.HitCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.KilledBy).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.Kills).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.MedalCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.PlayTime).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.RevengeCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.Score).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.SkillPoints).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponDamageGiven).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponDamageTakenBy).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponDeaths).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponFireCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponHeadshots).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponHitCount).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponKilledBy).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponKills).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponPlayTime).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponScore).HasDefaultValue(0);
            builder.Entity<DbCharacterStat>().Property(a => a.WeaponVehicleKills).HasDefaultValue(0);

            builder.Entity<DbCharacterStatByFaction>()
                .HasKey(a => new { a.CharacterId, a.ProfileId });
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.DominationCountVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.DominationCountNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.DominationCountTR).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.FacilityCaptureCountVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.FacilityCaptureCountNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.FacilityCaptureCountTR).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.KilledByVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.KilledByNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.KilledByTR).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.KillsVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.KillsNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.KillsTR).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.RevengeCountVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.RevengeCountNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.RevengeCountTR).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponDamageGivenVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponDamageGivenNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponDamageGivenTR).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponDamageTakenByVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponDamageTakenByNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponDamageTakenByTR).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponHeadshotsVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponHeadshotsNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponHeadshotsTR).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponKilledByVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponKilledByNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponKilledByTR).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponKillsVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponKillsNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponKillsTR).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponVehicleKillsVS).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponVehicleKillsNC).HasDefaultValue(0);
            builder.Entity<DbCharacterStatByFaction>().Property(a => a.WeaponVehicleKillsTR).HasDefaultValue(0);

            builder.Entity<DbCharacterWeaponStat>()
                .HasKey(a => new { a.CharacterId, a.ItemId, a.VehicleId });
            builder.Entity<DbCharacterWeaponStat>()
                .HasIndex(a => a.Kills);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.DamageGiven).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.DamageTakenBy).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.Deaths).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.FireCount).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.Headshots).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.HitCount).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.KilledBy).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.Kills).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.PlayTime).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.Score).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStat>().Property(a => a.VehicleKills).HasDefaultValue(0);


            builder.Entity<DbCharacterWeaponStatByFaction>()
                .HasKey(a => new { a.CharacterId, a.ItemId, a.VehicleId });
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.DamageGivenVS).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.DamageGivenNC).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.DamageGivenTR).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.DamageTakenByVS).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.DamageTakenByNC).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.DamageTakenByTR).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.HeadshotsVS).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.HeadshotsNC).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.HeadshotsTR).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.KilledByVS).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.KilledByNC).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.KilledByTR).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.KillsVS).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.KillsNC).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.KillsTR).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.VehicleKillsVS).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.VehicleKillsNC).HasDefaultValue(0);
            builder.Entity<DbCharacterWeaponStatByFaction>().Property(a => a.VehicleKillsTR).HasDefaultValue(0);

            builder.Entity<DbPlayerSession>()
                .HasIndex(a => new { a.CharacterId, a.LoginDate, a.LogoutDate });

            builder.Entity<DbVehicleFaction>()
                .HasKey(a => new { a.VehicleId, a.FactionId });

            builder.Entity<DbAlert>()
                .HasKey(a => new { a.WorldId, a.MetagameInstanceId });

            builder.Entity<DbEventAchievementEarned>()
                .HasKey(a => new { a.Timestamp, a.CharacterId });

            builder.Entity<DbEventBattlerankUp>()
                .HasKey(a => new { a.Timestamp, a.CharacterId });

            builder.Entity<DbEventContinentLock>()
                .HasKey(a => new { a.Timestamp, a.WorldId, a.ZoneId });

            builder.Entity<DbEventContinentUnlock>()
                .HasKey(a => new { a.Timestamp, a.WorldId, a.ZoneId });

            builder.Entity<DbEventDeath>()
                .HasKey(a => new { a.Timestamp, a.AttackerCharacterId, a.CharacterId });

            builder.Entity<DbEventFacilityControl>()
                .HasKey(a => new { a.Timestamp, a.WorldId, a.FacilityId });

            builder.Entity<DbEventFacilityControl>()
                .HasKey(a => new { a.Timestamp, a.WorldId, a.FacilityId });

            builder.Entity<DbEventGainExperience>()
                .HasKey(a => new { a.Timestamp, a.CharacterId, a.ExperienceId });

            builder.Entity<DbEventMetagameEvent>()
                .HasKey(a => new { a.MetagameId });

            builder.Entity<DbEventPlayerFacilityCapture>()
                .HasKey(a => new { a.Timestamp, a.CharacterId, a.FacilityId });

            builder.Entity<DbEventPlayerFacilityDefend>()
                .HasKey(a => new { a.Timestamp, a.CharacterId, a.FacilityId });

            builder.Entity<DbEventPlayerLogin>()
                .HasKey(a => new { a.Timestamp, a.CharacterId });

            builder.Entity<DbEventPlayerLogout>()
                .HasKey(a => new { a.Timestamp, a.CharacterId });

            builder.Entity<DbEventVehicleDestroy>()
                .HasKey(a => new { a.Timestamp, a.AttackerCharacterId, a.CharacterId });
        }
    }
}
