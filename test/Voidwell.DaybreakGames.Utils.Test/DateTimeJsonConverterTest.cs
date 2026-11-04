using System.Text.Json;
using Voidwell.DaybreakGames.Utils.JsonConverters;
using Xunit;

namespace Voidwell.DaybreakGames.Utils.Test;

// These must hold whatever the machine's time zone; run with TZ set (e.g. TZ=America/New_York) to check.
public class DateTimeJsonConverterTest
{
    private static readonly JsonSerializerOptions _options = new() { Converters = { new DateTimeJsonConverter() } };

    private static string Write(DateTime value) => JsonSerializer.Serialize(value, _options);

    private static DateTime Read(string json) => JsonSerializer.Deserialize<DateTime>(json, _options);

    [Fact]
    public void Write_TreatsUnspecifiedAsUtc()
    {
        // Dates read from `timestamp without time zone` columns come back unspecified but hold UTC.
        Assert.Equal("\"2026-11-03T16:18:17Z\"", Write(new DateTime(2026, 11, 3, 16, 18, 17, DateTimeKind.Unspecified)));
    }

    [Fact]
    public void Write_KeepsUtc()
    {
        Assert.Equal("\"2026-11-03T16:18:17Z\"", Write(new DateTime(2026, 11, 3, 16, 18, 17, DateTimeKind.Utc)));
    }

    [Fact]
    public void Write_ConvertsLocalToUtc()
    {
        var utc = new DateTime(2026, 11, 3, 16, 18, 17, DateTimeKind.Utc);

        Assert.Equal("\"2026-11-03T16:18:17Z\"", Write(utc.ToLocalTime()));
    }

    [Theory]
    [InlineData("\"2026-11-03T16:18:17Z\"")]
    [InlineData("\"2026-11-03T11:18:17-05:00\"")]
    [InlineData("\"2026-11-03T16:18:17\"")]
    [InlineData("\"2026-11-03 16:18:17.0\"")]
    public void Read_ReturnsUtc(string json)
    {
        var result = Read(json);

        Assert.Equal(DateTimeKind.Utc, result.Kind);
        Assert.Equal(new DateTime(2026, 11, 3, 16, 18, 17, DateTimeKind.Utc), result);
    }

    [Fact]
    public void RoundTrip_KeepsTheInstant()
    {
        var utc = new DateTime(2026, 11, 3, 16, 18, 17, DateTimeKind.Utc);

        Assert.Equal(utc, Read(Write(utc)));
    }
}
