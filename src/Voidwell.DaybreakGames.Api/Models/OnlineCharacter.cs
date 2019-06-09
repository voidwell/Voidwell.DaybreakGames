using System;

namespace Voidwell.DaybreakGames.Api.Models
{
    public class OnlineCharacter
    {
        public OnlineCharacterProfile Character { get; set; }
        public OnlineCharacterLastSeen LastSeen { get; set; }
        public DateTime LoginDate { get; set; }

        public void UpdateLastSeen(DateTime timestamp, int zoneId)
        {
            LastSeen = new OnlineCharacterLastSeen
            {
                Timestamp = timestamp,
                ZoneId = zoneId
            };
        }
    }

    public class OnlineCharacterProfile
    {
        public string CharacterId { get; set; }
        public int FactionId { get; set; }
        public string Name { get; set; }
        public int WorldId { get; set; }
    }

    public class OnlineCharacterLastSeen
    {
        public DateTime Timestamp { get; set; }
        public int ZoneId { get; set; }
    }
}
