using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class DailyWeaponStatsConfiguration : IEntityTypeConfiguration<DailyWeaponStats>
{
    public void Configure(EntityTypeBuilder<DailyWeaponStats> builder)
    {
        builder.ToTable("daily_weapon_stats");

        builder.HasKey(a => new { a.WeaponId, a.Date });

        builder.Property(a => a.Q1Kpu).HasColumnName("q1_kpu");
        builder.Property(a => a.Q2Kpu).HasColumnName("q2_kpu");
        builder.Property(a => a.Q3Kpu).HasColumnName("q3_kpu");
        builder.Property(a => a.Q4Kpu).HasColumnName("q4_kpu");
        builder.Property(a => a.Q4Kills).HasColumnName("q4_kills");
        builder.Property(a => a.Q4Uniques).HasColumnName("q4_uniques");
        builder.Property(a => a.Q4Headshots).HasColumnName("q4_headshots");
    }
}
