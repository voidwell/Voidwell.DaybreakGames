using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class GainExperience : IEntityTypeConfiguration<Models.Planetside.Events.GainExperience>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.GainExperience> builder)
        {
            builder.ToTable("EventGainExperience");

            builder.HasKey(a => new { a.Timestamp, a.CharacterId, a.ExperienceId });
        }
    }
}
