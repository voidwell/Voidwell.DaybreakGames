using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class DailyWeaponStatsConfiguration : IEntityTypeConfiguration<DailyWeaponStats>
    {
        public void Configure(EntityTypeBuilder<DailyWeaponStats> builder)
        {
            builder.ToTable("DailyWeaponStats");

            builder.HasKey(a => new { a.WeaponId, a.Date });
        }
    }
}
