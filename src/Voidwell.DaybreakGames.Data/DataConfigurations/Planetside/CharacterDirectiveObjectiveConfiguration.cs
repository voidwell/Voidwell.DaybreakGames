using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class CharacterDirectiveObjectiveConfiguration : IEntityTypeConfiguration<CharacterDirectiveObjective>
{
    public void Configure(EntityTypeBuilder<CharacterDirectiveObjective> builder)
    {
        builder.ToTable("character_directive_objective");

        builder.HasKey(a => new { a.CharacterId, a.DirectiveId, a.ObjectiveId });

        builder.Ignore(a => a.Objective);
    }
}
