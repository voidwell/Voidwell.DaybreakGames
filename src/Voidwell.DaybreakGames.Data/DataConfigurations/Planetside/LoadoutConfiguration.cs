using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class LoadoutConfiguration : IEntityTypeConfiguration<Loadout>
{
    public void Configure(EntityTypeBuilder<Loadout> builder)
    {
        builder.ToTable("loadout");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedNever();
    }
}
