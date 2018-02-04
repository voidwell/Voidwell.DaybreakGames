using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventPlayerFacilityDefendConfiguration : IEntityTypeConfiguration<EventPlayerFacilityDefend>
    {
        public void Configure(EntityTypeBuilder<EventPlayerFacilityDefend> builder)
        {
            builder.ToTable("EventPlayerFacilityDefend");

            builder.HasKey(a => new { a.Timestamp, a.CharacterId, a.FacilityId });
        }
    }
}
