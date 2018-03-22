using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class MapRegionConfiguration : IEntityTypeConfiguration<MapRegion>
    {
        public void Configure(EntityTypeBuilder<MapRegion> builder)
        {
            builder.ToTable("MapRegion");

            builder.HasKey(a => new { a.Id, a.FacilityId });

            builder.Property(a => a.Id).ValueGeneratedNever();
        }
    }
}
