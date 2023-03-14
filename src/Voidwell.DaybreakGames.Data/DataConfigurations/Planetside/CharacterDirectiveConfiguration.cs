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

            builder.Ignore(a => a.CharacterDirectiveObjectives)
                .Ignore(a => a.Directive);
        }
    }
}
