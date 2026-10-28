using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside.Events;

public class BattlerankUp : IEntityTypeConfiguration<Models.Planetside.Events.BattlerankUp>
{
    public void Configure(EntityTypeBuilder<Models.Planetside.Events.BattlerankUp> builder)
    {
        builder.ToTable("event_battlerank_up");

        builder.HasKey(a => new { a.CharacterId, a.Timestamp, a.BattleRank });
    }
}
