using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class MetagameEventStateConfiguration : IEntityTypeConfiguration<MetagameEventState>
{
    public void Configure(EntityTypeBuilder<MetagameEventState> builder)
    {
        builder.ToTable("MetagameEventState");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedNever();
    }
}
