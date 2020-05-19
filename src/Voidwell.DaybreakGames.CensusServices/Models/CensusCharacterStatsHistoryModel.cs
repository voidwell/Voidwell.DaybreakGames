namespace Voidwell.DaybreakGames.CensusServices.Models
{
    public class CensusCharacterStatsHistoryModel
    {
        public string CharacterId { get; set; }
        public string StatName { get; set; }
        public int AllTime { get; set; }
        public int OneLifeMax { get; set; }
        public StatHistoryDay Day { get; set; }
        public StatHistoryWeek Week { get; set; }
        public StatHistoryMonth Month { get; set; }
    }

    public class StatHistoryDay
    {
        public int d01 { get; set; }
        public int d02 { get; set; }
        public int d03 { get; set; }
        public int d04 { get; set; }
        public int d05 { get; set; }
        public int d06 { get; set; }
        public int d07 { get; set; }
        public int d08 { get; set; }
        public int d09 { get; set; }
        public int d10 { get; set; }
        public int d11 { get; set; }
        public int d12 { get; set; }
        public int d13 { get; set; }
        public int d14 { get; set; }
        public int d15 { get; set; }
        public int d16 { get; set; }
        public int d17 { get; set; }
        public int d18 { get; set; }
        public int d19 { get; set; }
        public int d20 { get; set; }
        public int d21 { get; set; }
        public int d22 { get; set; }
        public int d23 { get; set; }
        public int d24 { get; set; }
        public int d25 { get; set; }
        public int d26 { get; set; }
        public int d27 { get; set; }
        public int d28 { get; set; }
        public int d29 { get; set; }
        public int d30 { get; set; }
        public int d31 { get; set; }
    }

    public class StatHistoryWeek
    {
        public int w01 { get; set; }
        public int w02 { get; set; }
        public int w03 { get; set; }
        public int w04 { get; set; }
        public int w05 { get; set; }
        public int w06 { get; set; }
        public int w07 { get; set; }
        public int w08 { get; set; }
        public int w09 { get; set; }
        public int w10 { get; set; }
        public int w11 { get; set; }
        public int w12 { get; set; }
        public int w13 { get; set; }
    }

    public class StatHistoryMonth
    {
        public int m01 { get; set; }
        public int m02 { get; set; }
        public int m03 { get; set; }
        public int m04 { get; set; }
        public int m05 { get; set; }
        public int m06 { get; set; }
        public int m07 { get; set; }
        public int m08 { get; set; }
        public int m09 { get; set; }
        public int m10 { get; set; }
        public int m11 { get; set; }
        public int m12 { get; set; }
    }
}
