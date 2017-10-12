using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DBContext
{
    public class PS2DbContext : DbContextBase
    {
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
        public DbSet<DbEventDeath> EventDeaths { get; set; }
        public DbSet<DbEventVehicleDestroy> EventVehicleDestroys { get; set; }
        public DbSet<DbEventFacilityControl> EventFacilityControls { get; set; }
        public DbSet<DbPlayerSession> PlayerSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
