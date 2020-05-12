namespace Voidwell.DaybreakGames.Messaging.Models
{
    public class PlayerOnlineMessage : PlanetsideCharacterMessage
    {
        public PlayerOnlineMessage()
        {
            Type = "PlayerOnline";
        }

        public string CharacterName { get; set; }
        public string CharacterId { get; set; }
        public int FactionId { get; set; }
    }
}
