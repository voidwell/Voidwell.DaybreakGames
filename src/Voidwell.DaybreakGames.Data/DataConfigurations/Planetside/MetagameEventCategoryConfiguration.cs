using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class MetagameEventCategoryConfiguration : IEntityTypeConfiguration<MetagameEventCategory>
{
    public void Configure(EntityTypeBuilder<MetagameEventCategory> builder)
    {
        builder.ToTable("MetagameEventCategory");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedNever();
    }
}
