using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventAchievementEarnedConfiguration : IEntityTypeConfiguration<EventAchievementEarned>
    {
        public void Configure(EntityTypeBuilder<EventAchievementEarned> builder)
        {
            builder.ToTable("EventAchievementEarned");

            builder.HasKey(a => new { a.CharacterId, a.Timestamp });
        }
    }
}
