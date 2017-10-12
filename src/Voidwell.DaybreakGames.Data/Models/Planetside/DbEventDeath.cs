using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voidwell.DaybreakGames.Data.Models.Planetside
{
    [Table("EventDeath")]
    public class DbEventDeath
    {
        [Required]
        public string CharacterId { get; set; }
        [Required]
        public string AttackerCharacterId { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

        public string WorldId { get; set; }
        public string ZoneId { get; set; }
        public string CharacterLoadoutId { get; set; }
        public string CharacterOutfitId { get; set; }
        public string AttackerFireModeId { get; set; }
        public string AttackerLoadoutId { get; set; }
        public string AttackerVehicleId { get; set; }
        public string AttackerWeaponId { get; set; }
        public string AttackerOutfitId { get; set; }
        public bool IsHeadshot { get; set; }

        public DbCharacter Character { get; set; }
        public DbCharacter AttackerCharacter { get; set; }
        public DbOutfit CharacterOutfit { get; set; }
        public DbOutfit AttackerOutfit { get; set; }
        public DbVehicle AttackerVehicle { get; set; }
        public DbItem AttackerWeapon { get; set; }
    }
}
