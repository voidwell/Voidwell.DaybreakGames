using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class PlayerFacilityCapture : IEntityTypeConfiguration<Models.Planetside.Events.PlayerFacilityCapture>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.PlayerFacilityCapture> builder)
        {
            builder.ToTable("EventPlayerFacilityCapture");

            builder.HasKey(a => new { a.Timestamp, a.CharacterId, a.FacilityId });
        }
    }
}
