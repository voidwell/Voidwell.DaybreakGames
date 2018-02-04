using System;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.ResolvedModels
{
    public class EventDeath
    {
        public string CharacterId { get; set; }
        public string AttackerCharacterId { get; set; }
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

        public Character Character { get; set; }
        public Character AttackerCharacter { get; set; }
        public Outfit CharacterOutfit { get; set; }
        public Outfit AttackerOutfit { get; set; }
        public Vehicle AttackerVehicle { get; set; }
        public Item AttackerWeapon { get; set; }
    }
}
