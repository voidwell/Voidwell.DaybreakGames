using System;

namespace Voidwell.DaybreakGames.Models
{
    public class OutfitDetails
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public int? FactionId { get; set; }
        public string FactionName { get; set; }
        public int? WorldId { get; set; }
        public string WorldName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LeaderCharacterId { get; set; }
        public string LeaderName { get; set; }
        public int MemberCount { get; set; }
    }
}
