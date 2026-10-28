using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class CharacterDirectiveTreeConfiguration : IEntityTypeConfiguration<CharacterDirectiveTree>
{
    public void Configure(EntityTypeBuilder<CharacterDirectiveTree> builder)
    {
        builder.ToTable("character_directive_tree");

        builder.HasKey(a => new { a.CharacterId, a.DirectiveTreeId });

        builder.Ignore(a => a.CharacterDirectiveTiers);
    }
}
