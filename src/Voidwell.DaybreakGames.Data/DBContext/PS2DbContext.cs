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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
