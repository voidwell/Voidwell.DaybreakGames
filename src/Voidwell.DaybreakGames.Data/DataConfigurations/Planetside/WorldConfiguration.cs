using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class WorldConfiguration : IEntityTypeConfiguration<World>
{
    public void Configure(EntityTypeBuilder<World> builder)
    {
        builder.ToTable("world");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.Property(a => a.IsActive).HasDefaultValue(true).HasSentinel(true);
    }
}
