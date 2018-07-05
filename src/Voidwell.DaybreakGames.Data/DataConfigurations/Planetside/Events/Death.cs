using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Voidwell.DaybreakGames.Data.DataConfigurations
{
    internal class EventDeathConfiguration : IEntityTypeConfiguration<Models.Planetside.Events.Death>
    {
        public void Configure(EntityTypeBuilder<Models.Planetside.Events.Death> builder)
        {
            builder.ToTable("EventDeath");

            builder.HasKey(a => new { a.Timestamp, a.AttackerCharacterId, a.CharacterId });

            builder.HasIndex(a => new { a.AttackerWeaponId, a.Timestamp });

            builder
                .Ignore(a => a.Character)
                .Ignore(a => a.AttackerCharacter)
                .Ignore(a => a.CharacterOutfit)
                .Ignore(a => a.AttackerOutfit)
                .Ignore(a => a.AttackerVehicle)
                .Ignore(a => a.AttackerWeapon);
        }
    }
}
