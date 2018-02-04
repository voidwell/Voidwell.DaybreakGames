using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class FacilityLinkConfiguration : IEntityTypeConfiguration<FacilityLink>
    {
        public void Configure(EntityTypeBuilder<FacilityLink> builder)
        {
            builder.ToTable("FacilityLink");

            builder.HasKey(a => a.Id);
        }
    }
}
