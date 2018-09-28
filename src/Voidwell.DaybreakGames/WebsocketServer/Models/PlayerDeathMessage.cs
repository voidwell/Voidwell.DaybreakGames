namespace Voidwell.DaybreakGames.WebsocketServer.Models
{
    public class PlayerDeathMessage : PlanetsideMessage
    {
        public PlayerDeathMessage()
        {
            Type = "PlayerDeath";
        }

        public string AttackerCharacterId { get; set; }
        public string AttackerCharacterName { get; set; }
        public string VictimCharacterId { get; set; }
        public string VictimCharacterName { get; set; }
        public int? ZoneId { get; set; }
    }
}
