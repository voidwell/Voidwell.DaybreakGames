namespace Voidwell.DaybreakGames.WebsocketServer.Models
{
    public class PlayerOnlineMessage : PlanetsideMessage
    {
        public PlayerOnlineMessage()
        {
            Type = "PlayerOnline";
        }

        public string Name { get; set; }
    }
}
