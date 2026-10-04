namespace Voidwell.DaybreakGames.Domain.Models;

public class PlayerActivitySession
{
    public string? CharacterId { get; set; }
    public int? FactionId { get; set; }
    public DateTime? LoginDate { get; set; }
    public DateTime? LogoutDate { get; set; }
}
