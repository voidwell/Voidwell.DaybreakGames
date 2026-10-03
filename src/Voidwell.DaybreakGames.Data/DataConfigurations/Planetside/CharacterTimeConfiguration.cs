using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations;

public class CharacterTimeConfiguration : IEntityTypeConfiguration<CharacterTime>
{
    public void Configure(EntityTypeBuilder<CharacterTime> builder)
    {
        builder.ToTable("CharacterTime");

        builder.HasKey(a => a.CharacterId);

        builder.HasOne(a => a.Character)
            .WithOne(a => a.Time)
            .HasForeignKey<CharacterTime>(a => a.CharacterId);
    }
}
