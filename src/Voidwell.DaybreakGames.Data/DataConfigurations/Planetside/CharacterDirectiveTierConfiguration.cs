using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterDirectiveTierConfiguration : IEntityTypeConfiguration<CharacterDirectiveTier>
    {
        public void Configure(EntityTypeBuilder<CharacterDirectiveTier> builder)
        {
            builder.ToTable("CharacterDirectiveTier");

            builder.HasKey(a => new { a.CharacterId, a.DirectiveTreeId, a.DirectiveTierId });
        }
    }
}
