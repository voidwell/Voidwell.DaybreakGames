using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class GainExperience : IEntityTypeConfiguration<Models.Planetside.Events.GainExperience>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.GainExperience> builder)
        {
            builder.ToTable("EventGainExperience");

            builder.HasKey(a => new {a.Id, a.Timestamp, a.CharacterId, a.ExperienceId, a.OtherId });

            builder.HasIndex(a => new { a.WorldId, a.ZoneId, a.Timestamp });
        }
    }
}
