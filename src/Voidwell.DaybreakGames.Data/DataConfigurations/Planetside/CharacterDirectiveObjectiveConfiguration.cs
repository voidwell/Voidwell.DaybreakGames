using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterDirectiveObjectiveConfiguration : IEntityTypeConfiguration<CharacterDirectiveObjective>
    {
        public void Configure(EntityTypeBuilder<CharacterDirectiveObjective> builder)
        {
            builder.ToTable("CharacterDirectiveObjective");

            builder.HasKey(a => new { a.CharacterId, a.DirectiveId, a.ObjectiveId });

            builder.Ignore(a => a.Objective);
        }
    }
}
