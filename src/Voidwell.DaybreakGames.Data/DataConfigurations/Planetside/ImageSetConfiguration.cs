using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside
{
    public class ImageSetConfiguration : IEntityTypeConfiguration<ImageSet>
    {
        public void Configure(EntityTypeBuilder<ImageSet> builder)
        {
            builder.ToTable("ImageSet");

            builder.HasKey(a => new { a.Id, a.TypeId });

            builder.Property(a => a.Id).ValueGeneratedNever();
        }
    }
}
