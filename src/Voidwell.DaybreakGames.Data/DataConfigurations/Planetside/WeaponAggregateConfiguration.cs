using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class WeaponAggregateConfiguration : IEntityTypeConfiguration<WeaponAggregate>
    {
        public void Configure(EntityTypeBuilder<WeaponAggregate> builder)
        {
            builder.ToTable("WeaponAggregate");

            builder.HasKey(a => new { a.ItemId, a.VehicleId });
        }
    }
}
