using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterDirectiveConfiguration : IEntityTypeConfiguration<CharacterDirective>
    {
        public void Configure(EntityTypeBuilder<CharacterDirective> builder)
        {
            builder.ToTable("CharacterDirective");

            builder.HasKey(a => new { a.CharacterId, a.DirectiveId });

            builder.HasMany(a => a.CharacterDirectiveObjectives)
                .WithOne()
                .HasPrincipalKey(e => new { e.CharacterId, e.DirectiveId })
                .HasForeignKey(t => new { t.CharacterId, t.DirectiveId });
        }
    }
}
