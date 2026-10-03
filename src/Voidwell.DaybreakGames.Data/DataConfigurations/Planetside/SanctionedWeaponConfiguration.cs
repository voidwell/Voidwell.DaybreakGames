using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations;

public class SanctionedWeaponConfiguration : IEntityTypeConfiguration<SanctionedWeapon>
{
    public void Configure(EntityTypeBuilder<SanctionedWeapon> builder)
    {
        builder.ToTable("SanctionedWeapon");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedNever();
    }
}
