using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class ZoneOwnershipSnapshotConfiguration : IEntityTypeConfiguration<ZoneOwnershipSnapshot>
{
    public void Configure(EntityTypeBuilder<ZoneOwnershipSnapshot> builder)
    {
        builder.ToTable("ZoneOwnershipSnapshot");

        builder.HasKey(a => new { a.Timestamp, a.WorldId, a.ZoneId, a.RegionId });

        builder.HasIndex(a => new { a.WorldId, a.MetagameInstanceId });
    }
}
