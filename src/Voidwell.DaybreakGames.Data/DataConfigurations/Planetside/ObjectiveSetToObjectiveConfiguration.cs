using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations;

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
