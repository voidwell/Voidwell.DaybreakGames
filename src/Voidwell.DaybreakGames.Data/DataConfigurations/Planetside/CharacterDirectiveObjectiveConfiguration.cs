using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations;

public class CharacterDirectiveObjectiveConfiguration : IEntityTypeConfiguration<CharacterDirectiveObjective>
{
    public void Configure(EntityTypeBuilder<CharacterDirectiveObjective> builder)
    {
        builder.ToTable("CharacterDirectiveObjective");

        builder.HasKey(a => new { a.CharacterId, a.DirectiveId, a.ObjectiveId });

        builder.Ignore(a => a.Objective);
    }
}
