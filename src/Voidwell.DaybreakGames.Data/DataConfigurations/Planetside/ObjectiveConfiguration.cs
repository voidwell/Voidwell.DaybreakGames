using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class ObjectiveConfiguration : IEntityTypeConfiguration<Objective>
    {
        public void Configure(EntityTypeBuilder<Objective> builder)
        {
            builder.ToTable("Objective");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedNever();

            builder.Ignore(a => a.Achievement)
                .Ignore(a => a.Item);
        }
    }
}
