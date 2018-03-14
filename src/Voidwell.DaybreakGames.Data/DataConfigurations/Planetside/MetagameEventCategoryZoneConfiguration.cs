using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class MetagameEventCategoryZoneConfiguration : IEntityTypeConfiguration<MetagameEventCategoryZone>
    {
        public void Configure(EntityTypeBuilder<MetagameEventCategoryZone> builder)
        {
            builder.ToTable("MetagameEventCategoryZone");

            builder.HasKey(a => a.MetagameEventCategoryId);
        }
    }
}
