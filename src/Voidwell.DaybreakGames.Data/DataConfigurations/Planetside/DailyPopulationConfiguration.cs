using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class DailyPopulationConfiguration : IEntityTypeConfiguration<DailyPopulation>
{
    public void Configure(EntityTypeBuilder<DailyPopulation> builder)
    {
        builder.ToTable("DailyPopulation");

        builder.HasKey(a => new { a.Date, a.WorldId });
    }
}
