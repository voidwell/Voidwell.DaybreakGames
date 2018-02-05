using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class MetagameEventCategoryConfiguration : IEntityTypeConfiguration<MetagameEventCategory>
    {
        public void Configure(EntityTypeBuilder<MetagameEventCategory> builder)
        {
            builder.ToTable("MetagameEventCategory");

            builder.HasKey(a => a.Id);
        }
    }
}
