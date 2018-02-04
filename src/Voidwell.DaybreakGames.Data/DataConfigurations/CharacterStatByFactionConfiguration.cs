using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterStatByFactionConfiguration : IEntityTypeConfiguration<CharacterStatByFaction>
    {
        public void Configure(EntityTypeBuilder<CharacterStatByFaction> builder)
        {
            builder.ToTable("CharacterStatByFaction");

            builder.HasKey(a => new { a.CharacterId, a.ProfileId });

            builder.Ignore(a => a.Profile);

            builder.HasOne(a => a.Character)
                .WithMany(a => a.StatsByFaction)
                .HasForeignKey(a => a.CharacterId);

            builder.Property(a => a.KilledByVS).HasDefaultValue(0);
            builder.Property(a => a.KilledByNC).HasDefaultValue(0);
            builder.Property(a => a.KilledByTR).HasDefaultValue(0);
            builder.Property(a => a.KillsVS).HasDefaultValue(0);
            builder.Property(a => a.KillsNC).HasDefaultValue(0);
            builder.Property(a => a.KillsTR).HasDefaultValue(0);
        }
    }
}
