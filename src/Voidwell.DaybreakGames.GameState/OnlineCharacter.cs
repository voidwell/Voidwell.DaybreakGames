using System;

namespace Voidwell.DaybreakGames.GameState
{
    public class OnlineCharacter
    {
        public string CharacterId { get; internal set; }
        public string OutfitId { get; internal set; }
        public int FactionId { get; internal set; }
        public int WorldId { get; internal set; }
        public int? LastSeenZoneId { get; internal set; }
        public DateTime? LastSeenTimestamp { get; internal set; }
        public DateTime LoginTimestamp { get; internal set; }
        public DateTime? LogoutTimestamp { get; internal set; }
    }
}
