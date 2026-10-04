using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class WeaponAggregateConfiguration : IEntityTypeConfiguration<WeaponAggregate>
{
    public void Configure(EntityTypeBuilder<WeaponAggregate> builder)
    {
        builder.ToTable("WeaponAggregate");

        builder.HasKey(a => new { a.ItemId, a.VehicleId });
    }
}
