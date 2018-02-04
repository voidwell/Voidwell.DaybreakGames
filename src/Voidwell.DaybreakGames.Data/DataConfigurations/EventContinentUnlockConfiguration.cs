using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventContinentUnlockConfiguration : IEntityTypeConfiguration<EventContinentUnlock>
    {
        public void Configure(EntityTypeBuilder<EventContinentUnlock> builder)
        {
            builder.ToTable("EventContinentUnlock");

            builder.HasKey(a => new { a.Timestamp, a.WorldId, a.ZoneId });
        }
    }
}
