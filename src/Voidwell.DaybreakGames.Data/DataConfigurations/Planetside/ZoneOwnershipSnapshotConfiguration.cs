using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class ZoneOwnershipSnapshotConfiguration : IEntityTypeConfiguration<ZoneOwnershipSnapshot>
    {
        public void Configure(EntityTypeBuilder<ZoneOwnershipSnapshot> builder)
        {
            builder.ToTable("ZoneOwnershipSnapshot");

            builder.HasKey(a => new { a.Timestamp, a.WorldId, a.ZoneId, a.RegionId });

            builder.HasIndex(a => new { a.WorldId, a.MetagameInstanceId });
        }
    }
}
