using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class CharacterDirectiveTierConfiguration : IEntityTypeConfiguration<CharacterDirectiveTier>
{
    public void Configure(EntityTypeBuilder<CharacterDirectiveTier> builder)
    {
        builder.ToTable("character_directive_tier");

        builder.HasKey(a => new { a.CharacterId, a.DirectiveTreeId, a.DirectiveTierId });

        builder.Ignore(a => a.CharacterDirectives);
    }
}
