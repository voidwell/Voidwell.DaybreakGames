using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations.Planetside
{
    public class DirectiveTreeCategoryConfiguration : IEntityTypeConfiguration<DirectiveTreeCategory>
    {
        public void Configure(EntityTypeBuilder<DirectiveTreeCategory> builder)
        {
            builder.ToTable("DirectiveTreeCategory");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).ValueGeneratedNever();

            builder.Ignore(a => a.Trees);
        }
    }
}
