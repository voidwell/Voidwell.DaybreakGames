namespace Voidwell.DaybreakGames.Data.Repositories.Models;

public class CharacterLastSession
{
    public long CharacterId { get; set; }
    public string? Name { get; set; }
    public int? SessionId { get; set; }
    public DateTime? LoginDate { get; set; }
    public DateTime? LogoutDate { get; set; }
    public int? Duration { get; set; }
}
