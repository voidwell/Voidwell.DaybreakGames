using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class CharacterStatConfiguration : IEntityTypeConfiguration<CharacterStat>
    {
        public void Configure(EntityTypeBuilder<CharacterStat> builder)
        {
            builder.ToTable("CharacterStat");

            builder.HasKey(a => new { a.CharacterId, a.ProfileId });

            builder.Ignore(a => a.Profile);

            builder.HasOne(a => a.Character)
                .WithMany(a => a.Stats)
                .HasForeignKey(a => a.CharacterId);

            builder.Property(a => a.Deaths).HasDefaultValue(0);
            builder.Property(a => a.FireCount).HasDefaultValue(0);
            builder.Property(a => a.HitCount).HasDefaultValue(0);
            builder.Property(a => a.KilledBy).HasDefaultValue(0);
            builder.Property(a => a.Kills).HasDefaultValue(0);
            builder.Property(a => a.PlayTime).HasDefaultValue(0);
            builder.Property(a => a.Score).HasDefaultValue(0);
        }
    }
}
