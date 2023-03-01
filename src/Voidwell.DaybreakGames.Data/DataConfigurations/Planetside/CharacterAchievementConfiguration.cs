using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside
{ 
    public class CharacterAchievementConfiguration : IEntityTypeConfiguration<CharacterAchievement>
    {
        public void Configure(EntityTypeBuilder<CharacterAchievement> builder)
        {
            builder.ToTable("CharacterAchievement");

            builder.HasKey(a => new { a.CharacterId, a.AchievementId });

            builder.Ignore(a => a.Achievement);
        }
    }
}
