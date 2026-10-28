using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class RewardSetToRewardGroupConfiguration : IEntityTypeConfiguration<RewardSetToRewardGroup>
{
    public void Configure(EntityTypeBuilder<RewardSetToRewardGroup> builder)
    {
        builder.ToTable("reward_set_to_reward_group");

        builder.HasKey(a => new { a.RewardSetId, a.RewardGroupId });

        builder.Ignore(a => a.RewardGroups);
    }
}
