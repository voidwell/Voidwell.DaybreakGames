using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class PlayerLogin : IEntityTypeConfiguration<Models.Planetside.Events.PlayerLogin>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.PlayerLogin> builder)
        {
            builder.ToTable("EventPlayerLogin");

            builder.HasKey(a => new { a.Timestamp, a.CharacterId });
        }
    }
}
