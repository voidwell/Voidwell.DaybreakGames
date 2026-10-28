using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside.Events;

public class AchievementEarned : IEntityTypeConfiguration<Models.Planetside.Events.AchievementEarned>
{
    public void Configure(EntityTypeBuilder<Models.Planetside.Events.AchievementEarned> builder)
    {
        builder.ToTable("event_achievement_earned");

        builder.HasKey(a => new { a.CharacterId, a.Timestamp });
    }
}
