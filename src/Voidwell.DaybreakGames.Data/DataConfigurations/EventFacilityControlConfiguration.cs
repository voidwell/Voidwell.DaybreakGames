using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventFacilityControlConfiguration : IEntityTypeConfiguration<EventFacilityControl>
    {
        public void Configure(EntityTypeBuilder<EventFacilityControl> builder)
        {
            builder.ToTable("EventFacilityControl");

            builder.HasKey(a => new { a.Timestamp, a.WorldId, a.FacilityId });
        }
    }
}
