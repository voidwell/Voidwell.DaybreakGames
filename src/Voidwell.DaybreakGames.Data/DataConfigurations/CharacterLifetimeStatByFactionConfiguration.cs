using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterLifetimeStatByFactionConfiguration : IEntityTypeConfiguration<CharacterLifetimeStatByFaction>
    {
        public void Configure(EntityTypeBuilder<CharacterLifetimeStatByFaction> builder)
        {
            builder.ToTable("CharacterLifetimeStatByFaction");

            builder.HasKey(a => a.CharacterId);

            builder.HasOne(a => a.Character)
                .WithOne(a => a.LifetimeStatsByFaction)
                .HasForeignKey<CharacterLifetimeStatByFaction>(a => a.CharacterId);

            builder.Property(a => a.DominationCountVS).HasDefaultValue(0);
            builder.Property(a => a.DominationCountNC).HasDefaultValue(0);
            builder.Property(a => a.DominationCountTR).HasDefaultValue(0);
            builder.Property(a => a.FacilityCaptureCountVS).HasDefaultValue(0);
            builder.Property(a => a.FacilityCaptureCountNC).HasDefaultValue(0);
            builder.Property(a => a.FacilityCaptureCountTR).HasDefaultValue(0);
            builder.Property(a => a.RevengeCountVS).HasDefaultValue(0);
            builder.Property(a => a.RevengeCountNC).HasDefaultValue(0);
            builder.Property(a => a.RevengeCountTR).HasDefaultValue(0);
            builder.Property(a => a.WeaponDamageGivenVS).HasDefaultValue(0);
            builder.Property(a => a.WeaponDamageGivenNC).HasDefaultValue(0);
            builder.Property(a => a.WeaponDamageGivenTR).HasDefaultValue(0);
            builder.Property(a => a.WeaponDamageTakenByVS).HasDefaultValue(0);
            builder.Property(a => a.WeaponDamageTakenByNC).HasDefaultValue(0);
            builder.Property(a => a.WeaponDamageTakenByTR).HasDefaultValue(0);
            builder.Property(a => a.WeaponHeadshotsVS).HasDefaultValue(0);
            builder.Property(a => a.WeaponHeadshotsNC).HasDefaultValue(0);
            builder.Property(a => a.WeaponHeadshotsTR).HasDefaultValue(0);
            builder.Property(a => a.WeaponKilledByVS).HasDefaultValue(0);
            builder.Property(a => a.WeaponKilledByNC).HasDefaultValue(0);
            builder.Property(a => a.WeaponKilledByTR).HasDefaultValue(0);
            builder.Property(a => a.WeaponKillsVS).HasDefaultValue(0);
            builder.Property(a => a.WeaponKillsNC).HasDefaultValue(0);
            builder.Property(a => a.WeaponKillsTR).HasDefaultValue(0);
            builder.Property(a => a.WeaponVehicleKillsVS).HasDefaultValue(0);
            builder.Property(a => a.WeaponVehicleKillsNC).HasDefaultValue(0);
            builder.Property(a => a.WeaponVehicleKillsTR).HasDefaultValue(0);
        }
    }
}
