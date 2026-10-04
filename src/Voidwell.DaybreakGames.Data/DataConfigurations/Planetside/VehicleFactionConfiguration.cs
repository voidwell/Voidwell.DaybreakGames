using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

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
