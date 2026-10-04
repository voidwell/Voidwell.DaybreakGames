using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class MapRegionConfiguration : IEntityTypeConfiguration<MapRegion>
{
    public void Configure(EntityTypeBuilder<MapRegion> builder)
    {
        builder.ToTable("MapRegion");

        builder.HasKey(a => new { a.Id });

        builder.Property(a => a.Id).ValueGeneratedNever();
    }
}
