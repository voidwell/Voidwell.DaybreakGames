using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside
{
    public class DirectiveTreeConfiguration : IEntityTypeConfiguration<DirectiveTree>
    {
        public void Configure(EntityTypeBuilder<DirectiveTree> builder)
        {
            builder.ToTable("DirectiveTree");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedNever();

            builder.Ignore(a => a.Tiers);
        }
    }
}
