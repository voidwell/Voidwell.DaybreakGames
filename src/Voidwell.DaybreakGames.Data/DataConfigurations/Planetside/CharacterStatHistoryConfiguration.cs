using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside;

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
