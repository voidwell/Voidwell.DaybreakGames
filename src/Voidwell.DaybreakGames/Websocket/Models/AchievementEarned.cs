namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class AchievementEarned : PayloadBase
    {
        public string CharacterId { get; set; }
        public string AchievementId { get; set; }
    }
}
