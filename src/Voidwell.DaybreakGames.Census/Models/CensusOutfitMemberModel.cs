namespace Voidwell.DaybreakGames.Census.Models;

public class CensusOutfitMemberModel
{
    public long CharacterId { get; set; }
    public long OutfitId { get; set; }
    public DateTime MemberSinceDate { get; set; }
    public string? Rank { get; set; }
    public int RankOrdinal { get; set; }
}
