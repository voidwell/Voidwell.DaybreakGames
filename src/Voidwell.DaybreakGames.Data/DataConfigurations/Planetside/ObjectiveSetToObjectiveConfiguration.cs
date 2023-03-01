using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class ObjectiveSetToObjectiveConfiguration : IEntityTypeConfiguration<ObjectiveSetToObjective>
    {
        public void Configure(EntityTypeBuilder<ObjectiveSetToObjective> builder)
        {
            builder.ToTable("ObjectiveSetToObjective");

            builder.HasKey(a => a.ObjectiveSetId);

            builder.Property(a => a.ObjectiveSetId).ValueGeneratedNever();

            builder.Ignore(a => a.Objectives);
        }
    }
}
