namespace Voidwell.DaybreakGames.Services.Models
{
    public class RatingCharacterModel
    {
        public string CharacterId { get; set; }
        public string Name { get; set; }
        public int? FactionId { get; set; }
        public int? WorldId { get; set; }
        public int? BattleRank { get; set; }
        public double Rating { get; set; }
        public double Deviation { get; set; }
    }
}
