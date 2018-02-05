using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class PlayerFacilityDefend : IEntityTypeConfiguration<Models.Planetside.Events.PlayerFacilityDefend>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.PlayerFacilityDefend> builder)
        {
            builder.ToTable("EventPlayerFacilityDefend");

            builder.HasKey(a => new { a.Timestamp, a.CharacterId, a.FacilityId });
        }
    }
}
