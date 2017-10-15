namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class ContinentLock : PayloadBase
    {
        public string TriggeringFaction { get; set; }
        public string MetagameEventId { get; set; }
        public float VsPopulation { get; set; }
        public float NcPopulation { get; set; }
        public float TrPopulation { get; set; }
    }
}
