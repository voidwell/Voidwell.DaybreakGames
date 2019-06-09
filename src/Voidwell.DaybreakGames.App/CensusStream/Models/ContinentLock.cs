namespace Voidwell.DaybreakGames.CensusStream.Models
{
    public class ContinentLock : PayloadBase
    {
        public int TriggeringFaction { get; set; }
        public int MetagameEventId { get; set; }
        public float VsPopulation { get; set; }
        public float NcPopulation { get; set; }
        public float TrPopulation { get; set; }
    }
}
