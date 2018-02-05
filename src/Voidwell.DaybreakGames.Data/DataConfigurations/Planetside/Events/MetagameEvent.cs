using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class MetagameEvent : IEntityTypeConfiguration<Models.Planetside.Events.MetagameEvent>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.MetagameEvent> builder)
        {
            builder.ToTable("EventMetagameEvent");

            builder.HasKey(a => a.MetagameId);
        }
    }
}
