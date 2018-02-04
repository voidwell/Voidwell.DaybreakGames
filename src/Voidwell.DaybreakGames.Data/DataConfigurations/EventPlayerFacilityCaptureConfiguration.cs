using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventPlayerFacilityCaptureConfiguration : IEntityTypeConfiguration<EventPlayerFacilityCapture>
    {
        public void Configure(EntityTypeBuilder<EventPlayerFacilityCapture> builder)
        {
            builder.ToTable("EventPlayerFacilityCapture");

            builder.HasKey(a => new { a.Timestamp, a.CharacterId, a.FacilityId });
        }
    }
}
