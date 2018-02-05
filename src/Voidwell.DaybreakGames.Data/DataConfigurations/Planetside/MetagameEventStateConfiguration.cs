using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class MetagameEventStateConfiguration : IEntityTypeConfiguration<MetagameEventState>
    {
        public void Configure(EntityTypeBuilder<MetagameEventState> builder)
        {
            builder.ToTable("MetagameEventState");

            builder.HasKey(a => a.Id);
        }
    }
}
