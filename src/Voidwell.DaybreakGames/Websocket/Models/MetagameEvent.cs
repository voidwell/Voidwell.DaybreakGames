namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class MetagameEvent : PayloadBase
    {
        public string InstanceId { get; set; }
        public string MetagameEventId { get; set; }
        public string MetagameEventState { get; set; }
        public float FactionVs { get; set; }
        public float FactionNc { get; set; }
        public float FactionTr { get; set; }
        public int ExperienceBonus { get; set; }
    }
}
