using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventContinentLockConfiguration : IEntityTypeConfiguration<EventContinentLock>
    {
        public void Configure(EntityTypeBuilder<EventContinentLock> builder)
        {
            builder.ToTable("EventContinentLock");

            builder.HasKey(a => new { a.Timestamp, a.WorldId, a.ZoneId });
        }
    }
}
