namespace Voidwell.DaybreakGames.Domain.Models;

public class PlayerSessionSummary
{
    public string Id { get; set; } = null!;
    public int Duration { get; set; }
    public DateTime LoginDate { get; set; }
    public DateTime? LogoutDate { get; set; }
    public int Kills { get; set; }
    public int Deaths { get; set; }
}
