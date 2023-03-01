using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class RewardConfiguration : IEntityTypeConfiguration<Reward>
    {
        public void Configure(EntityTypeBuilder<Reward> builder)
        {
            builder.ToTable("Reward");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedNever();

            builder.Ignore(a => a.Item);
        }
    }
}
