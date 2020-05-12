namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusFacilityWorldEventModel
    {
        public string EventType { get; set; }
        public string TableType { get; set; }
        public int Timestamp { get; set; }
        public int ZoneId { get; set; }
        public int WorldId { get; set; }

        public int FacilityId { get; set; }
        public int FactionOld { get; set; }
        public int FactionNew { get; set; }
        public int DurationHeld { get; set; }
        public int ObjectiveId { get; set; }
        public string OutfitId { get; set; }
    }
}
