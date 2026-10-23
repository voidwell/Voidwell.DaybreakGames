namespace Voidwell.DaybreakGames.Census.Models;

public class CensusCharacterNameModel
{
    public long CharacterId { get; set; }
    public string? First { get; set; }
    public string? FirstLower { get; set; }
}

public class CensusCharacterName
{
    public string? First { get; set; }
    public string? FirstLower { get; set; }
}
