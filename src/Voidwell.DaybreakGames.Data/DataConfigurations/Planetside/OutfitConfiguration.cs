using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations;

public class OutfitConfiguration : IEntityTypeConfiguration<Outfit>
{
    public void Configure(EntityTypeBuilder<Outfit> builder)
    {
        builder.ToTable("Outfit");

        builder.HasKey(a => a.Id);

        builder.Ignore(a => a.LeaderCharacter)
               .Ignore(a => a.Faction)
               .Ignore(a => a.World);
    }
}
