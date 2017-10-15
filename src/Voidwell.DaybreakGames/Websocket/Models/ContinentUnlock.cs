namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class ContinentUnlock : PayloadBase
    {
        public string TriggeringFaction { get; set; }
        public string MetagameEventId { get; set; }
    }
}
