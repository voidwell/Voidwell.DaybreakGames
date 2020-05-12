namespace Voidwell.DaybreakGames.GameState.CensusStream.Models
{
    public class BattlerankUp : PayloadBase
    {
        public string CharacterId { get; set; }
        public int BattleRank { get; set; }
    }
}
