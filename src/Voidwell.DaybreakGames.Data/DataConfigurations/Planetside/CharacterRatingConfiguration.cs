using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations;

public class CharacterRatingConfiguration : IEntityTypeConfiguration<CharacterRating>
{
    public void Configure(EntityTypeBuilder<CharacterRating> builder)
    {
        builder.ToTable("CharacterRating");

        builder.HasKey(a => a.CharacterId);

        builder.HasIndex(a => new { a.Rating });

        builder.HasOne(a => a.Character)
            .WithOne(a => a.Rating)
            .HasForeignKey<CharacterRating>(a => a.CharacterId);
    }
}
