using System;

namespace Voidwell.DaybreakGames.App.Models
{
    public class PlayerActivitySession
    {
        public string CharacterId { get; set; }
        public int? FactionId { get; set; }
        public DateTime? LoginDate { get; set; }
        public DateTime? LogoutDate { get; set; }
    }
}
