using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventMetagameEventConfiguration : IEntityTypeConfiguration<EventMetagameEvent>
    {
        public void Configure(EntityTypeBuilder<EventMetagameEvent> builder)
        {
            builder.ToTable("EventMetagameEvent");

            builder.HasKey(a => new { a.MetagameId });
        }
    }
}
