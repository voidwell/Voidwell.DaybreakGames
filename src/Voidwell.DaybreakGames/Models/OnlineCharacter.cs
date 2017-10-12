using System;

namespace Voidwell.DaybreakGames.Models
{
    public class OnlineCharacter
    {
        public OnlineCharacterProfile Character { get; set; }
        public DateTime LoginDate { get; set; }
    }

    public class OnlineCharacterProfile
    {
        public string CharacterId { get; set; }
        public string FactionId { get; set; }
        public string Name { get; set; }
        public string WorldId { get; set; }
    }
}
