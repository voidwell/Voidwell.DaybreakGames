using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Census.Models;

public class CensusCharacterStatsHistoryModel
{
    public string? CharacterId { get; set; }
    public string? StatName { get; set; }
    public int AllTime { get; set; }
    public int OneLifeMax { get; set; }
    public StatHistoryDay? Day { get; set; }
    public StatHistoryWeek? Week { get; set; }
    public StatHistoryMonth? Month { get; set; }
}

public class StatHistoryDay
{
    [JsonPropertyName("d01")]
    public int D01 { get; set; }
    [JsonPropertyName("d02")]
    public int D02 { get; set; }
    [JsonPropertyName("d03")]
    public int D03 { get; set; }
    [JsonPropertyName("d04")]
    public int D04 { get; set; }
    [JsonPropertyName("d05")]
    public int D05 { get; set; }
    [JsonPropertyName("d06")]
    public int D06 { get; set; }
    [JsonPropertyName("d07")]
    public int D07 { get; set; }
    [JsonPropertyName("d08")]
    public int D08 { get; set; }
    [JsonPropertyName("d09")]
    public int D09 { get; set; }
    [JsonPropertyName("d10")]
    public int D10 { get; set; }
    [JsonPropertyName("d11")]
    public int D11 { get; set; }
    [JsonPropertyName("d12")]
    public int D12 { get; set; }
    [JsonPropertyName("d13")]
    public int D13 { get; set; }
    [JsonPropertyName("d14")]
    public int D14 { get; set; }
    [JsonPropertyName("d15")]
    public int D15 { get; set; }
    [JsonPropertyName("d16")]
    public int D16 { get; set; }
    [JsonPropertyName("d17")]
    public int D17 { get; set; }
    [JsonPropertyName("d18")]
    public int D18 { get; set; }
    [JsonPropertyName("d19")]
    public int D19 { get; set; }
    [JsonPropertyName("d20")]
    public int D20 { get; set; }
    [JsonPropertyName("d21")]
    public int D21 { get; set; }
    [JsonPropertyName("d22")]
    public int D22 { get; set; }
    [JsonPropertyName("d23")]
    public int D23 { get; set; }
    [JsonPropertyName("d24")]
    public int D24 { get; set; }
    [JsonPropertyName("d25")]
    public int D25 { get; set; }
    [JsonPropertyName("d26")]
    public int D26 { get; set; }
    [JsonPropertyName("d27")]
    public int D27 { get; set; }
    [JsonPropertyName("d28")]
    public int D28 { get; set; }
    [JsonPropertyName("d29")]
    public int D29 { get; set; }
    [JsonPropertyName("d30")]
    public int D30 { get; set; }
    [JsonPropertyName("d31")]
    public int D31 { get; set; }
}

public class StatHistoryWeek
{
    [JsonPropertyName("w01")]
    public int W01 { get; set; }
    [JsonPropertyName("w02")]
    public int W02 { get; set; }
    [JsonPropertyName("w03")]
    public int W03 { get; set; }
    [JsonPropertyName("w04")]
    public int W04 { get; set; }
    [JsonPropertyName("w05")]
    public int W05 { get; set; }
    [JsonPropertyName("w06")]
    public int W06 { get; set; }
    [JsonPropertyName("w07")]
    public int W07 { get; set; }
    [JsonPropertyName("w08")]
    public int W08 { get; set; }
    [JsonPropertyName("w09")]
    public int W09 { get; set; }
    [JsonPropertyName("w10")]
    public int W10 { get; set; }
    [JsonPropertyName("w11")]
    public int W11 { get; set; }
    [JsonPropertyName("w12")]
    public int W12 { get; set; }
    [JsonPropertyName("w13")]
    public int W13 { get; set; }
}

public class StatHistoryMonth
{
    [JsonPropertyName("m01")]
    public int M01 { get; set; }
    [JsonPropertyName("m02")]
    public int M02 { get; set; }
    [JsonPropertyName("m03")]
    public int M03 { get; set; }
    [JsonPropertyName("m04")]
    public int M04 { get; set; }
    [JsonPropertyName("m05")]
    public int M05 { get; set; }
    [JsonPropertyName("m06")]
    public int M06 { get; set; }
    [JsonPropertyName("m07")]
    public int M07 { get; set; }
    [JsonPropertyName("m08")]
    public int M08 { get; set; }
    [JsonPropertyName("m09")]
    public int M09 { get; set; }
    [JsonPropertyName("m10")]
    public int M10 { get; set; }
    [JsonPropertyName("m11")]
    public int M11 { get; set; }
    [JsonPropertyName("m12")]
    public int M12 { get; set; }
}
