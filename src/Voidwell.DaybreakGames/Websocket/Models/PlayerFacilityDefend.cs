namespace Voidwell.DaybreakGames.Websocket.Models
{
    public class PlayerFacilityDefend : PayloadBase
    {
        public string CharacterId { get; set; }
        public string FacilityId { get; set; }
        public string OutfitId { get; set; }
    }
}
