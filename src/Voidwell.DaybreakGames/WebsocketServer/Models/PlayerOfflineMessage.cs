namespace Voidwell.DaybreakGames.WebsocketServer.Models
{
    public class PlayerOfflineMessage : PlanetsideMessage
    {
        public PlayerOfflineMessage()
        {
            Type = "PlayerOffline";
        }

        public string Name { get; set; }
    }
}
