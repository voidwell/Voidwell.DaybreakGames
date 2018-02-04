using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventBattlerankUpConfiguration : IEntityTypeConfiguration<EventBattlerankUp>
    {
        public void Configure(EntityTypeBuilder<EventBattlerankUp> builder)
        {
            builder.ToTable("EventBattlerankUp");

            builder.HasKey(a => new { a.CharacterId, a.Timestamp });
        }
    }
}
