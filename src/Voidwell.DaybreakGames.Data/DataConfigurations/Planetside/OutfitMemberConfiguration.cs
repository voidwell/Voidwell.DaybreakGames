using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

public class OutfitMemberConfiguration : IEntityTypeConfiguration<OutfitMember>
{
    public void Configure(EntityTypeBuilder<OutfitMember> builder)
    {
        builder.ToTable("OutfitMember");

        builder.HasKey(a => a.CharacterId);

        builder.Ignore(a => a.Outfit);

        builder.HasOne(a => a.Character)
            .WithOne(a => a.OutfitMembership)
            .HasForeignKey<OutfitMember>(a => a.CharacterId);
    }
}
