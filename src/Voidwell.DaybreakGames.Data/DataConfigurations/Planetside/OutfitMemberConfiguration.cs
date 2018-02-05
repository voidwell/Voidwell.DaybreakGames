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

            builder.HasKey(a => new { a.CharacterId, a.OutfitId });

            builder.HasOne(a => a.Character)
                .WithOne()
                .HasForeignKey<OutfitMember>(a => a.CharacterId);

            builder.HasOne(a => a.Outfit)
                .WithMany()
                .HasForeignKey(a => a.OutfitId);
        }
    }
}
