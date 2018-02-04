using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventGainExperienceConfiguration : IEntityTypeConfiguration<EventGainExperience>
    {
        public void Configure(EntityTypeBuilder<EventGainExperience> builder)
        {
            builder.ToTable("EventGainExperience");

            builder.HasKey(a => new { a.Timestamp, a.CharacterId, a.ExperienceId });
        }
    }
}
