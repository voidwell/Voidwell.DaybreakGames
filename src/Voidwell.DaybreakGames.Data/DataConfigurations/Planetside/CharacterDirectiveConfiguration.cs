using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class CharacterDirectiveConfiguration : IEntityTypeConfiguration<CharacterDirective>
{
    public void Configure(EntityTypeBuilder<CharacterDirective> builder)
    {
        builder.ToTable("CharacterDirective");

        builder.HasKey(a => new { a.CharacterId, a.DirectiveId });

        builder.Ignore(a => a.CharacterDirectiveObjectives)
            .Ignore(a => a.Directive);
    }
}
