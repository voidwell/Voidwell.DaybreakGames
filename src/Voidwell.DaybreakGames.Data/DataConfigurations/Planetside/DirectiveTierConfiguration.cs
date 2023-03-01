using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside
{
    public class DirectiveTierConfiguration : IEntityTypeConfiguration<DirectiveTier>
    {
        public void Configure(EntityTypeBuilder<DirectiveTier> builder)
        {
            builder.ToTable("DirectiveTier");

            builder.HasKey(a => new { a.DirectiveTreeId, a.DirectiveTierId });

            builder.Ignore(a => a.Directives)
                .Ignore(a => a.RewardGroupSets);
        }
    }
}
