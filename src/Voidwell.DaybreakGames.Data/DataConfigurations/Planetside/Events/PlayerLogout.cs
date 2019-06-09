using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class PlayerLogout : IEntityTypeConfiguration<Models.Planetside.Events.PlayerLogout>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.PlayerLogout> builder)
        {
            builder.ToTable("EventPlayerLogout");

            builder.HasKey(a => new { a.Timestamp, a.CharacterId });

            builder.HasIndex(a => new { a.Timestamp, a.WorldId });
        }
    }
}
