namespace Voidwell.DaybreakGames.Live.CensusStream.Models;

public class PlayerFacilityDefend : PayloadBase
{
    public long CharacterId { get; set; }
    public int FacilityId { get; set; }
    public long? OutfitId { get; set; }
}
