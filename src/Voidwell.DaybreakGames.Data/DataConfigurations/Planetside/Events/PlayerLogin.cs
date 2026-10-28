using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside.Events;

public class PlayerLogin : IEntityTypeConfiguration<Models.Planetside.Events.PlayerLogin>
{
    public void Configure(EntityTypeBuilder<Models.Planetside.Events.PlayerLogin> builder)
    {
        builder.ToTable("event_player_login");

        builder.HasKey(a => new { a.Timestamp, a.CharacterId });

        builder.HasIndex(a => new { a.Timestamp, a.WorldId });
    }
}
