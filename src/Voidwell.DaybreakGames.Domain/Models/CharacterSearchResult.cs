using System;

namespace Voidwell.DaybreakGames.Domain.Models
{
    public class CharacterSearchResult
    {
        public string Name { get; set; }
        public int BattleRank { get; set; }
        public string Id { get; set; }
        public int FactionId { get; set; }
        public DateTime LastLogin { get; set; }
        public int WorldId { get; set; }
        public bool OnlineStatus { get; set; }
    }
}
