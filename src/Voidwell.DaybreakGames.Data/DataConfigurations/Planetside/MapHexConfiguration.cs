using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class MapHexConfiguration : IEntityTypeConfiguration<MapHex>
    {
        public void Configure(EntityTypeBuilder<MapHex> builder)
        {
            builder.ToTable("MapHex");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedOnAdd();
        }
    }
}
