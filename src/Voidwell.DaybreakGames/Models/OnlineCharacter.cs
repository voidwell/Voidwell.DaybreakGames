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
        public int FactionId { get; set; }
        public string Name { get; set; }
        public int WorldId { get; set; }
    }
}
