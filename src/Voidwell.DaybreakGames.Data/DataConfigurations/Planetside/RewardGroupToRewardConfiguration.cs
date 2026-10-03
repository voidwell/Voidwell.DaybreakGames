using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations;

public class RewardGroupToRewardConfiguration : IEntityTypeConfiguration<RewardGroupToReward>
{
    public void Configure(EntityTypeBuilder<RewardGroupToReward> builder)
    {
        builder.ToTable("RewardGroupToReward");

        builder.HasKey(a => new { a.RewardGroupId, a.RewardId });

        builder.Ignore(a => a.Reward);
    }
}
