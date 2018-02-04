using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventPlayerLogoutConfiguration : IEntityTypeConfiguration<EventPlayerLogout>
    {
        public void Configure(EntityTypeBuilder<EventPlayerLogout> builder)
        {
            builder.ToTable("EventPlayerLogout");

            builder.HasKey(a => new { a.Timestamp, a.CharacterId });
        }
    }
}
