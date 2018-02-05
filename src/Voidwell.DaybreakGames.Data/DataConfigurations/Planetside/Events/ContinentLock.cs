using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class ContinentLock : IEntityTypeConfiguration<Models.Planetside.Events.ContinentLock>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.ContinentLock> builder)
        {
            builder.ToTable("EventContinentLock");

            builder.HasKey(a => new { a.Timestamp, a.WorldId, a.ZoneId });
        }
    }
}
