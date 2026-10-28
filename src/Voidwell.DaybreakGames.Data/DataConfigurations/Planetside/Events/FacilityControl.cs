using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside.Events;

public class FacilityControl : IEntityTypeConfiguration<Models.Planetside.Events.FacilityControl>
{
    public void Configure(EntityTypeBuilder<Models.Planetside.Events.FacilityControl> builder)
    {
        builder.ToTable("event_facility_control");

        builder.HasKey(a => new { a.Timestamp, a.WorldId, a.FacilityId, a.NewFactionId });
    }
}
