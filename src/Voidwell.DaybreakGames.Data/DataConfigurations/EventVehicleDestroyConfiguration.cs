using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class EventVehicleDestroyConfiguration : IEntityTypeConfiguration<EventVehicleDestroy>
    {
        public void Configure(EntityTypeBuilder<EventVehicleDestroy> builder)
        {
            builder.ToTable("EventVehicleDestroy");

            builder.HasKey(a => new { a.Timestamp, a.AttackerCharacterId, a.CharacterId });
        }
    }
}
