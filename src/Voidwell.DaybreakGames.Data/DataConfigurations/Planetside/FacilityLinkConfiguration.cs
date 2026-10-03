using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations;

public class FacilityLinkConfiguration : IEntityTypeConfiguration<FacilityLink>
{
    public void Configure(EntityTypeBuilder<FacilityLink> builder)
    {
        builder.ToTable("FacilityLink");

        builder.HasKey(a => new { a.FacilityIdA, a.FacilityIdB });
    }
}
