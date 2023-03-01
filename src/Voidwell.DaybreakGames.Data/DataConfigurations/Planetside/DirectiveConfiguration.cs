using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside
{
    public class DirectiveConfiguration : IEntityTypeConfiguration<Directive>
    {
        public void Configure(EntityTypeBuilder<Directive> builder)
        {
            builder.ToTable("Directive");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedNever();

            builder.Ignore(a => a.ObjectiveSet);
        }
    }
}
