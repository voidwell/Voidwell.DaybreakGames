using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class MetagameEventCategoryZoneConfiguration : IEntityTypeConfiguration<MetagameEventCategoryZone>
{
    public void Configure(EntityTypeBuilder<MetagameEventCategoryZone> builder)
    {
        builder.ToTable("metagame_event_category_zone");

        builder.HasKey(a => a.MetagameEventCategoryId);

        builder.Property(a => a.MetagameEventCategoryId).ValueGeneratedNever();
    }
}
