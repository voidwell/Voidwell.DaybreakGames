using System.Text.Json;
using Voidwell.DaybreakGames.Live.CensusStream;
using Voidwell.DaybreakGames.Live.CensusStream.JsonConverters;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Live.Test;

public class StreamJsonTest
{
    private record BoolHolder(bool Value);
    private record DateHolder(DateTime Value);

    private static readonly JsonSerializerOptions _converterOptions = new()
    {
        Converters = { new BooleanJsonConverter(), new DateTimeJsonConverter() }
    };

    [Theory]
    [InlineData("\"true\"", true)]
    [InlineData("\"True\"", true)]
    [InlineData("\"1\"", true)]
    [InlineData("\"false\"", false)]
    [InlineData("\"0\"", false)]
    [InlineData("\"anything else\"", false)]
    public void BooleanJsonConverter_ReadsCensusStyleStrings(string json, bool expected)
    {
        Assert.Equal(expected, JsonSerializer.Deserialize<bool>(json, _converterOptions));
    }

    [Fact]
    public void BooleanJsonConverter_WritesJsonBoolean()
    {
        Assert.Equal("true", JsonSerializer.Serialize(true, _converterOptions));
        Assert.Equal("false", JsonSerializer.Serialize(false, _converterOptions));
    }

    [Fact]
    public void DateTimeJsonConverter_ReadsEpochSecondsAsUtc()
    {
        var result = JsonSerializer.Deserialize<DateTime>("\"1700000000\"", _converterOptions);

        Assert.Equal(new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc), result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void DateTimeJsonConverter_ReadsIsoStringsAsUtc()
    {
        var result = JsonSerializer.Deserialize<DateTime>("\"2024-03-05T10:20:30\"", _converterOptions);

        Assert.Equal(new DateTime(2024, 3, 5, 10, 20, 30, DateTimeKind.Utc), result);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }

    [Fact]
    public void DateTimeJsonConverter_WritesUtcIsoString()
    {
        var json = JsonSerializer.Serialize(new DateTime(2024, 3, 5, 10, 20, 30, DateTimeKind.Utc), _converterOptions);

        Assert.Equal("\"2024-03-05T10:20:30Z\"", json);
    }

    [Theory]
    [InlineData("EventName", "event_name")]
    [InlineData("WorldId", "world_id")]
    [InlineData("CharacterId", "character_id")]
    [InlineData("Timestamp", "timestamp")]
    [InlineData("ZoneId", "zone_id")]
    public void UnderscorePropertyJsonNamingPolicy_ConvertsPascalCaseToSnakeCase(string name, string expected)
    {
        Assert.Equal(expected, new UnderscorePropertyJsonNamingPolicy().ConvertName(name));
    }

    [Fact]
    public void UnderscorePropertyJsonNamingPolicy_NullName_Throws()
    {
        var act = () => new UnderscorePropertyJsonNamingPolicy().ConvertName(null);

        Assert.ThrowsAny<ArgumentNullException>(act);
    }

    [Fact]
    public void SerializerOptions_DeserializesCensusPayloadWithStringEncodedValues()
    {
        const string json = """
            {"character_id":"5428","event_name":"PlayerLogin","timestamp":"1700000000","world_id":"17","zone_id":"2"}
            """;

        var payload = JsonSerializer.Deserialize<PlayerLogin>(json, StreamConstants.SerializerOptions);

        Assert.Equal(5428, payload.CharacterId);
        Assert.Equal("PlayerLogin", payload.EventName);
        Assert.Equal(17, payload.WorldId);
        Assert.Equal(2, payload.ZoneId);
        Assert.Equal(new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc), payload.Timestamp);
    }

    [Fact]
    public void SerializerOptions_PayloadWithoutZone_HasNullZoneId()
    {
        const string json = """{"event_name":"PlayerLogin","timestamp":"1700000000","world_id":"17"}""";

        Assert.Null(JsonSerializer.Deserialize<PayloadBase>(json, StreamConstants.SerializerOptions).ZoneId);
    }
}
