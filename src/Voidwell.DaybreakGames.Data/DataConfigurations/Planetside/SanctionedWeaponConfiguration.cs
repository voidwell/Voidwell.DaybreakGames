using Microsoft.EntityFrameworkCore;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    public class SanctionedWeaponConfiguration : IEntityTypeConfiguration<SanctionedWeapon>
    {
        public void Configure(EntityTypeBuilder<SanctionedWeapon> builder)
        {
            builder.ToTable("SanctionedWeapon");

            builder.HasKey(a => a.Id);
        }
    }
}
