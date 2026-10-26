namespace Voidwell.DaybreakGames.Census.Models;

public class CensusOutfitModel
{
    public long OutfitId { get; set; }
    public string? Name { get; set; }
    public string? NameLower { get; set; }
    public string? Alias { get; set; }
    public string? AliasLower { get; set; }
    public DateTime TimeCreated { get; set; }
    public long? LeaderCharacterId { get; set; }
    public int MemberCount { get; set; }
}
