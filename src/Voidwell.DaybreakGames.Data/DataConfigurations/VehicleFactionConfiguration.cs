using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class VehicleFactionConfiguration : IEntityTypeConfiguration<VehicleFaction>
    {
        public void Configure(EntityTypeBuilder<VehicleFaction> builder)
        {
            builder.ToTable("VehicleFaction");

            builder.HasKey(a => new { a.VehicleId, a.FactionId });

            builder.HasOne(a => a.Vehicle)
                .WithMany(a => a.Faction)
                .HasForeignKey(a => a.VehicleId);
        }
    }
}
