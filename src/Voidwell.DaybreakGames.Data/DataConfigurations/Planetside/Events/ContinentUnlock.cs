using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class ContinentUnlock : IEntityTypeConfiguration<Models.Planetside.Events.ContinentUnlock>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.ContinentUnlock> builder)
        {
            builder.ToTable("EventContinentUnlock");

            builder.HasKey(a => new { a.Timestamp, a.WorldId, a.ZoneId });
        }
    }
}
