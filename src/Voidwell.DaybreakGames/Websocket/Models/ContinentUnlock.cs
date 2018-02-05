namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class ContinentUnlock : PayloadBase
    {
        public int TriggeringFaction { get; set; }
        public int MetagameEventId { get; set; }
    }
}
