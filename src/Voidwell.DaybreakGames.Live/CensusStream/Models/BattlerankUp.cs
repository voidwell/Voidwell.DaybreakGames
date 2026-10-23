namespace Voidwell.DaybreakGames.Live.CensusStream.Models;

public class BattlerankUp : PayloadBase
{
    public long CharacterId { get; set; }
    public int BattleRank { get; set; }
}
