using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations;

public class UpdaterScheduler : IEntityTypeConfiguration<Models.UpdaterScheduler>
{
    public void Configure(EntityTypeBuilder<Models.UpdaterScheduler> builder)
    {
        builder.ToTable("updater_scheduler");

        builder.HasKey(a => a.Id);
    }
}
