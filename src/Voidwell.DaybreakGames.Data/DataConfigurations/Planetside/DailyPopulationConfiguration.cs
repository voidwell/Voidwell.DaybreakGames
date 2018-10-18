using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class DailyPopulationConfiguration : IEntityTypeConfiguration<DailyPopulation>
    {
        public void Configure(EntityTypeBuilder<DailyPopulation> builder)
        {
            builder.ToTable("DailyPopulation");

            builder.HasKey(a => new { a.Date, a.WorldId });
        }
    }
}
