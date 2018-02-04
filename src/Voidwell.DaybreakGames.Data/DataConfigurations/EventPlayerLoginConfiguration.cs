using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventPlayerLoginConfiguration : IEntityTypeConfiguration<EventPlayerLogin>
    {
        public void Configure(EntityTypeBuilder<EventPlayerLogin> builder)
        {
            builder.ToTable("EventPlayerLogin");

            builder.HasKey(a => new { a.Timestamp, a.CharacterId });
        }
    }
}
