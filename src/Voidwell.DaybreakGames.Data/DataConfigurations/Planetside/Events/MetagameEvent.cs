using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside.Events;

public class MetagameEvent : IEntityTypeConfiguration<Models.Planetside.Events.MetagameEvent>
{
    public void Configure(EntityTypeBuilder<Models.Planetside.Events.MetagameEvent> builder)
    {
        builder.ToTable("event_metagame_event");

        builder.HasKey(a => a.MetagameId);

        builder.Property(a => a.MetagameId).ValueGeneratedOnAdd();
    }
}
