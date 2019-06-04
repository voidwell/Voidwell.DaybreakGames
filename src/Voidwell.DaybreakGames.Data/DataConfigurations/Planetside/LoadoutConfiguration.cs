using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class LoadoutConfiguration : IEntityTypeConfiguration<Loadout>
    {
        public void Configure(EntityTypeBuilder<Loadout> builder)
        {
            builder.ToTable("Loadout");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedNever();
        }
    }
}
