namespace Voidwell.DaybreakGames.Messaging.Models
{
    public class PlanetsideAlertMessage : PlanetsideMessage
    {
        public int InstanceId { get; set; }
        public int MetagameEventId { get; set; }
        public int MetagameInstanceId { get; set; }
        public string MetagameEventDescription { get; set; }
        public float ScoreFactionVS { get; set; }
        public float ScoreFactionNC { get; set; }
        public float ScoreFactionTR { get; set; }
        public float ScoreFactionNS { get; set; }
    }
}
