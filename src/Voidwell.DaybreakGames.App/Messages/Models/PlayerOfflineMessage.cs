namespace Voidwell.DaybreakGames.Messages.Models
{
    public class PlayerOfflineMessage : PlanetsideCharacterMessage
    {
        public PlayerOfflineMessage()
        {
            Type = "PlayerOffline";
        }

        public string CharacterName { get; set; }
        public string CharacterId { get; set; }
        public int FactionId { get; set; }
    }
}
