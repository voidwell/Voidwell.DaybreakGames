namespace Voidwell.DaybreakGames.Api.Models
{
    public class SearchResult
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public int BattleRank { get; set; }
        public int? FactionId { get; set; }
        public int? WorldId { get; set; }
        public string Alias { get; set; }
        public int MemberCount { get; set; }
        public string CategoryId { get; set; }
    }
}
