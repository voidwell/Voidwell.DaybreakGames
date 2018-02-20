using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class EventVehicleDestroyConfiguration : IEntityTypeConfiguration<Models.Planetside.Events.VehicleDestroy>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.VehicleDestroy> builder)
        {
            builder.ToTable("EventVehicleDestroy");

            builder.HasKey(a => new { a.Timestamp, a.AttackerCharacterId, a.CharacterId, a.AttackerVehicleId, a.VehicleId });
        }
    }
}
