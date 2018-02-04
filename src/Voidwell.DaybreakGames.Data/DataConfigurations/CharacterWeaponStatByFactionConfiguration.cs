using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterWeaponStatByFactionConfiguration : IEntityTypeConfiguration<CharacterWeaponStatByFaction>
    {
        public void Configure(EntityTypeBuilder<CharacterWeaponStatByFaction> builder)
        {
            builder.ToTable("CharacterWeaponStatByFaction");

            builder.HasKey(a => new { a.CharacterId, a.ItemId, a.VehicleId });

            builder
                .Ignore(a => a.Item)
                .Ignore(a => a.Vehicle);

            builder.HasOne(a => a.Character)
                .WithMany(a => a.WeaponStatsByFaction)
                .HasForeignKey(a => a.CharacterId);

            builder.Property(a => a.DamageGivenVS).HasDefaultValue(0);
            builder.Property(a => a.DamageGivenNC).HasDefaultValue(0);
            builder.Property(a => a.DamageGivenTR).HasDefaultValue(0);
            builder.Property(a => a.DamageTakenByVS).HasDefaultValue(0);
            builder.Property(a => a.DamageTakenByNC).HasDefaultValue(0);
            builder.Property(a => a.DamageTakenByTR).HasDefaultValue(0);
            builder.Property(a => a.HeadshotsVS).HasDefaultValue(0);
            builder.Property(a => a.HeadshotsNC).HasDefaultValue(0);
            builder.Property(a => a.HeadshotsTR).HasDefaultValue(0);
            builder.Property(a => a.KilledByVS).HasDefaultValue(0);
            builder.Property(a => a.KilledByNC).HasDefaultValue(0);
            builder.Property(a => a.KilledByTR).HasDefaultValue(0);
            builder.Property(a => a.KillsVS).HasDefaultValue(0);
            builder.Property(a => a.KillsNC).HasDefaultValue(0);
            builder.Property(a => a.KillsTR).HasDefaultValue(0);
            builder.Property(a => a.VehicleKillsVS).HasDefaultValue(0);
            builder.Property(a => a.VehicleKillsNC).HasDefaultValue(0);
            builder.Property(a => a.VehicleKillsTR).HasDefaultValue(0);
        }
    }
}
