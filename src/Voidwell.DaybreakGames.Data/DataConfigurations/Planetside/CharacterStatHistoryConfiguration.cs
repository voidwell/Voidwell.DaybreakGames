using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterStatHistoryConfiguration : IEntityTypeConfiguration<CharacterStatHistory>
    {
        public void Configure(EntityTypeBuilder<CharacterStatHistory> builder)
        {
            builder.ToTable("CharacterStatHistory");

            builder.HasKey(a => new { a.CharacterId, a.StatName });

            builder.HasOne(a => a.Character)
                .WithMany(a => a.StatsHistory)
                .HasForeignKey(a => a.CharacterId);
        }
    }
}
