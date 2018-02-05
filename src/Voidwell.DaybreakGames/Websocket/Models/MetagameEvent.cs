namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class MetagameEvent : PayloadBase
    {
        public int InstanceId { get; set; }
        public int MetagameEventId { get; set; }
        public int MetagameEventState { get; set; }
        public float FactionVs { get; set; }
        public float FactionNc { get; set; }
        public float FactionTr { get; set; }
        public int ExperienceBonus { get; set; }
    }
}
