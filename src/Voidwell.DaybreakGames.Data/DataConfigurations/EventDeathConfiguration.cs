using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventDeathConfiguration : IEntityTypeConfiguration<EventDeath>
    {
        public void Configure(EntityTypeBuilder<EventDeath> builder)
        {
            builder.ToTable("EventDeath");

            builder.HasKey(a => new { a.Timestamp, a.AttackerCharacterId, a.CharacterId });
        }
    }
}
