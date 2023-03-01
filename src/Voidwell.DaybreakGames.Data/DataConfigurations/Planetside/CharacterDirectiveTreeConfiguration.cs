using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterDirectiveTreeConfiguration : IEntityTypeConfiguration<CharacterDirectiveTree>
    {
        public void Configure(EntityTypeBuilder<CharacterDirectiveTree> builder)
        {
            builder.ToTable("CharacterDirectiveTree");

            builder.HasKey(a => new { a.CharacterId, a.DirectiveTreeId });

            builder.HasMany(a => a.CharacterDirectiveTiers)
                .WithOne()
                .HasPrincipalKey(e => new { e.CharacterId, e.DirectiveTreeId })
                .HasForeignKey(t => new { t.CharacterId, t.DirectiveTreeId });

            builder.HasMany(a => a.CharacterDirectives)
                .WithOne()
                .HasPrincipalKey(e => new { e.CharacterId, e.DirectiveTreeId })
                .HasForeignKey(t => new { t.CharacterId, t.DirectiveTreeId });
        }
    }
}
