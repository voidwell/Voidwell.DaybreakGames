namespace Voidwell.DaybreakGames.Live.CensusStream.Models
{
    public class PlayerFacilityCapture : PayloadBase
    {
        public string CharacterId { get; set; }
        public int FacilityId { get; set; }
        public string OutfitId { get; set; }
    }
}
