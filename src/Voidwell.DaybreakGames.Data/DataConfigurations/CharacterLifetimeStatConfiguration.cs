using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterLifetimeStatConfiguration : IEntityTypeConfiguration<CharacterLifetimeStat>
    {
        public void Configure(EntityTypeBuilder<CharacterLifetimeStat> builder)
        {
            builder.ToTable("CharacterLifetimeStat");

            builder.HasKey(a => a.CharacterId);

            builder.HasOne(a => a.Character)
                .WithOne(a => a.LifetimeStats)
                .HasForeignKey<CharacterLifetimeStat>(a => a.CharacterId);

            builder.Property(a => a.AchievementCount).HasDefaultValue(0);
            builder.Property(a => a.AssistCount).HasDefaultValue(0);
            builder.Property(a => a.DominationCount).HasDefaultValue(0);
            builder.Property(a => a.FacilityCaptureCount).HasDefaultValue(0);
            builder.Property(a => a.FacilityDefendedCount).HasDefaultValue(0);
            builder.Property(a => a.MedalCount).HasDefaultValue(0);
            builder.Property(a => a.RevengeCount).HasDefaultValue(0);
            builder.Property(a => a.SkillPoints).HasDefaultValue(0);
            builder.Property(a => a.WeaponDamageGiven).HasDefaultValue(0);
            builder.Property(a => a.WeaponDamageTakenBy).HasDefaultValue(0);
            builder.Property(a => a.WeaponDeaths).HasDefaultValue(0);
            builder.Property(a => a.WeaponFireCount).HasDefaultValue(0);
            builder.Property(a => a.WeaponHeadshots).HasDefaultValue(0);
            builder.Property(a => a.WeaponHitCount).HasDefaultValue(0);
            builder.Property(a => a.WeaponKills).HasDefaultValue(0);
            builder.Property(a => a.WeaponPlayTime).HasDefaultValue(0);
            builder.Property(a => a.WeaponScore).HasDefaultValue(0);
            builder.Property(a => a.WeaponVehicleKills).HasDefaultValue(0);
        }
    }
}
