namespace Voidwell.DaybreakGames.Census.Models
{
    public class CensusMetagameWorldEventModel
    {
        public string EventType { get; set; }
        public string TableType { get; set; }
        public int Timestamp { get; set; }
        public int ZoneId { get; set; }
        public int WorldId { get; set; }

        public int MetagameEventId { get; set; }
        public int MetagameEventState { get; set; }
        public int FactionNc { get; set; }
        public int FactionTr { get; set; }
        public int FactionVs { get; set; }
        public int ExperienceBonus { get; set; }
        public int InstanceId { get; set; }
        public string MetagameEventStateName { get; set; }
    }
}
