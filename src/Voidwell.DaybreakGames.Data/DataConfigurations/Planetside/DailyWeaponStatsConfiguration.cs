using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class DailyWeaponStatsConfiguration : IEntityTypeConfiguration<DailyWeaponStats>
{
    public void Configure(EntityTypeBuilder<DailyWeaponStats> builder)
    {
        builder.ToTable("DailyWeaponStats");

        builder.HasKey(a => new { a.WeaponId, a.Date });
    }
}
