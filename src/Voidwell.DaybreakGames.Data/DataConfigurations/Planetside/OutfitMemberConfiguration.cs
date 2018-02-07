using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
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
}
