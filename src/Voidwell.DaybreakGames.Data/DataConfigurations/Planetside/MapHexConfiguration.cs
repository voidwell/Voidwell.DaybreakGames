using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class MapHexConfiguration : IEntityTypeConfiguration<MapHex>
{
    public void Configure(EntityTypeBuilder<MapHex> builder)
    {
        builder.ToTable("MapHex");

        builder.HasKey(a => new { a.MapRegionId, a.XPos, a.YPos, a.ZoneId });
    }
}
