namespace Voidwell.DaybreakGames.Live.CensusStream.Models;

public class AchievementEarned : PayloadBase
{
    public long CharacterId { get; set; }
    public int AchievementId { get; set; }
}
