using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Models
{
    public class PlayerSession
    {
        public IEnumerable<PlayerSessionEvent> Events { get; set; }
        public PlayerSessionInfo Session { get; set; }
    }

    public class PlayerSessionEvent
    {
        public DateTime Timestamp { get; set; }
        public CombatReportItemDetail Attacker { get; set; }
        public CombatReportItemDetail Victim { get; set; }
        public PlayerSessionWeapon Weapon { get; set; }
        public string ZoneId { get; set; }
        public string AttackerFireModeId { get; set; }
        public string AttackerLoadoutId { get; set; }
        public string AttackerOutfitId { get; set; }
        public string AttackerVehicleId { get; set; }
        public string CharacterLoadoutId { get; set; }
        public string CharacterOutfitId { get; set; }
        public bool IsHeadshot { get; set; }
    }

    public class PlayerSessionInfo
    {
        public string CharacterId { get; set; }
        public int Duration { get; set; } 
        public string Id { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime LogoutDate { get; set; }
    }

    public class PlayerSessionWeapon
    {
        public string Id { get; set; }
        public string ImageId { get; set; }
        public string Name { get; set; }
    }
}
