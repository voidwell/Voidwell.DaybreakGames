namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class FacilityControl : PayloadBase
    {
        public string FacilityId { get; set; }
        public string NewFactionId { get; set; }
        public string OldFactionId { get; set; }
        public int DurationHeld { get; set; }
        public string OutfitId { get; set; }
    }
}
