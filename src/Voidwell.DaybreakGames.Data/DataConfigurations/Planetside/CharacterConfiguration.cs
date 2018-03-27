using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterConfiguration : IEntityTypeConfiguration<Character>
    {
        public void Configure(EntityTypeBuilder<Character> builder)
        {
            builder.ToTable("Character");

            builder.HasKey(a => a.Id);

            builder.HasIndex(a => a.Name);

            builder
                .Ignore(a => a.Title)
                .Ignore(a => a.World)
                .Ignore(a => a.Faction);
        }
    }
}
