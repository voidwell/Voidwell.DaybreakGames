namespace Voidwell.DaybreakGames.Live.CensusStream.Models;

public class PlayerFacilityCapture : PayloadBase
{
    public long CharacterId { get; set; }
    public int FacilityId { get; set; }
    public long? OutfitId { get; set; }
}
