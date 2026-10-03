using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.DataConfigurations;

public class PlayerSessionConfiguration : IEntityTypeConfiguration<PlayerSession>
{
    public void Configure(EntityTypeBuilder<PlayerSession> builder)
    {
        builder.ToTable("PlayerSession");

        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.CharacterId, a.LoginDate, a.LogoutDate });

        builder.Property(a => a.Id).ValueGeneratedOnAdd();
    }
}
