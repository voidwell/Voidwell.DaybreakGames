using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterWeaponStatConfiguration : IEntityTypeConfiguration<CharacterWeaponStat>
    {
        public void Configure(EntityTypeBuilder<CharacterWeaponStat> builder)
        {
            builder.ToTable("CharacterWeaponStat");

            builder.HasKey(a => new { a.CharacterId, a.ItemId, a.VehicleId });

            builder
                .Ignore(a => a.Item)
                .Ignore(a => a.Vehicle);

            builder.HasOne(a => a.Character)
                .WithMany(a => a.WeaponStats)
                .HasForeignKey(a => a.CharacterId);

            builder.HasIndex(a => a.Kills);

            builder.Property(a => a.DamageGiven).HasDefaultValue(0);
            builder.Property(a => a.DamageTakenBy).HasDefaultValue(0);
            builder.Property(a => a.Deaths).HasDefaultValue(0);
            builder.Property(a => a.FireCount).HasDefaultValue(0);
            builder.Property(a => a.Headshots).HasDefaultValue(0);
            builder.Property(a => a.HitCount).HasDefaultValue(0);
            builder.Property(a => a.KilledBy).HasDefaultValue(0);
            builder.Property(a => a.Kills).HasDefaultValue(0);
            builder.Property(a => a.PlayTime).HasDefaultValue(0);
            builder.Property(a => a.Score).HasDefaultValue(0);
            builder.Property(a => a.VehicleKills).HasDefaultValue(0);
        }
    }
}
