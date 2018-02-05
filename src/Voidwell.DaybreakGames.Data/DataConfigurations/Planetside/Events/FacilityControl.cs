using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class FacilityControl : IEntityTypeConfiguration<Models.Planetside.Events.FacilityControl>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.FacilityControl> builder)
        {
            builder.ToTable("EventFacilityControl");

            builder.HasKey(a => new { a.Timestamp, a.WorldId, a.FacilityId });
        }
    }
}
