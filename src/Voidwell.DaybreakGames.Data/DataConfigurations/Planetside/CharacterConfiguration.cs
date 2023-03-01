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

            builder.Property(a => a.PrestigeLevel).HasDefaultValue(0);

            builder
                .Ignore(a => a.Title)
                .Ignore(a => a.World)
                .Ignore(a => a.Faction);

            builder.HasMany(e => e.DirectiveTrees)
                .WithOne()
                .HasPrincipalKey(e => e.Id)
                .HasForeignKey(t => t.CharacterId);
        }
    }
}
