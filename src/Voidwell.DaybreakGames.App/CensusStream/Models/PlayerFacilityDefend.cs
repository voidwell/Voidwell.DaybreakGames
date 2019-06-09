namespace Voidwell.DaybreakGames.CensusStream.Models
{
    public class PlayerFacilityDefend : PayloadBase
    {
        public string CharacterId { get; set; }
        public int FacilityId { get; set; }
        public string OutfitId { get; set; }
    }
}
