using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class UpdaterScheduler : IEntityTypeConfiguration<Models.UpdaterScheduler>
    {
        public void Configure(EntityTypeBuilder<Models.UpdaterScheduler> builder)
        {
            builder.ToTable("UpdaterScheduler");

            builder.HasKey(a => a.Id);
        }
    }
}
