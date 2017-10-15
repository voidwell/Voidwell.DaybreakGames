namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class GainExperience : PayloadBase
    {
        public string CharacterId { get; set; }
        public string ExperienceId { get; set; }
        public int Amount { get; set; }
        public string LoadoutId { get; set; }
        public string OtherId { get; set; }
    }
}
