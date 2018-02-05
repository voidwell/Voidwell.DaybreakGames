using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class AchievementEarned : IEntityTypeConfiguration<Models.Planetside.Events.AchievementEarned>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.AchievementEarned> builder)
        {
            builder.ToTable("EventAchievementEarned");

            builder.HasKey(a => new { a.CharacterId, a.Timestamp });
        }
    }
}
