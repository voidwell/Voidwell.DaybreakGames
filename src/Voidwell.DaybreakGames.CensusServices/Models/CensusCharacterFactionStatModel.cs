namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusCharacterFactionStatModel
    {
        public string CharacterId { get; set; }
        public string StatName { get; set; }
        public int ProfileId { get; set; }
        public int ValueForeverVs { get; set; }
        public int ValueForeverNc { get; set; }
        public int ValueForeverTr { get; set; }
        public int ValueMonthlyVs { get; set; }
        public int ValueMonthlyNc { get; set; }
        public int ValueMonthlyTr { get; set; }
        public int ValueWeeklyVs { get; set; }
        public int ValueWeeklyNc { get; set; }
        public int ValueWeeklyTr { get; set; }
        public int ValueDailyVs { get; set; }
        public int ValueDailyNc { get; set; }
        public int ValueDailyTr { get; set; }
    }
}
