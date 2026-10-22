namespace Voidwell.DaybreakGames.Data.Models.Planetside;

public class CharacterRating
{
    public long CharacterId { get; set; }
    public double Rating { get; set; }
    public double Deviation { get; set; }
    public double Volatility { get; set; }

    public Character? Character { get; set; }
}
