using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class BattlerankUp : IEntityTypeConfiguration<Models.Planetside.Events.BattlerankUp>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.BattlerankUp> builder)
        {
            builder.ToTable("EventBattlerankUp");

            builder.HasKey(a => new { a.CharacterId, a.Timestamp });
        }
    }
}
