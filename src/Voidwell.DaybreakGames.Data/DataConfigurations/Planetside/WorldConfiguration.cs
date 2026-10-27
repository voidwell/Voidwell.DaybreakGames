using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class WorldConfiguration : IEntityTypeConfiguration<World>
{
    public void Configure(EntityTypeBuilder<World> builder)
    {
        builder.ToTable("World");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.Property(a => a.IsActive).HasDefaultValue(true).HasSentinel(true);
        builder.Property(a => a.Platform).HasConversion<string>().HasMaxLength(16).HasDefaultValue(Platform.PC).HasSentinel((Platform)(-1));
    }
}
