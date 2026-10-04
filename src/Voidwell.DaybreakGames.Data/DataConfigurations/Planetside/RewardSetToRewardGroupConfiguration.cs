using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class RewardSetToRewardGroupConfiguration : IEntityTypeConfiguration<RewardSetToRewardGroup>
{
    public void Configure(EntityTypeBuilder<RewardSetToRewardGroup> builder)
    {
        builder.ToTable("RewardSetToRewardGroup");

        builder.HasKey(a => new { a.RewardSetId, a.RewardGroupId });

        builder.Ignore(a => a.RewardGroups);
    }
}
