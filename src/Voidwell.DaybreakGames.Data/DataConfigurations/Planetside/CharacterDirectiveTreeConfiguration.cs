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

            builder.Ignore(a => a.CharacterDirectiveTiers);
        }
    }
}
