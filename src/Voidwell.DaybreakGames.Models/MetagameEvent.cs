namespace Voidwell.DaybreakGames.Models
{
    public class MetagameEvent : PayloadBase
    {
        public int InstanceId { get; set; }
        public int MetagameEventId { get; set; }
        public string MetagameEventState { get; set; }
        public float FactionVs { get; set; }
        public float FactionNc { get; set; }
        public float FactionTr { get; set; }
        public double ExperienceBonus { get; set; }
    }
}
