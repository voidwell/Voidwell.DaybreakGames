namespace Voidwell.DaybreakGames.Live.CensusStream.Models
{
    public class GainExperience : PayloadBase
    {
        public string CharacterId { get; set; }
        public int ExperienceId { get; set; }
        public int Amount { get; set; }
        public int? LoadoutId { get; set; }
        public string OtherId { get; set; }
    }
}
