using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class RewardSetToRewardGroupConfiguration : IEntityTypeConfiguration<RewardSetToRewardGroup>
    {
        public void Configure(EntityTypeBuilder<RewardSetToRewardGroup> builder)
        {
            builder.ToTable("RewardSetToRewardGroup");

            builder.HasKey(a => new { a.RewardSetId, a.RewardGroupId });

            builder.Ignore(a => a.RewardGroups);
        }
    }
}
