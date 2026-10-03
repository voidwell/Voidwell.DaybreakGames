using System.Text.Json;
using FluentAssertions;
using Voidwell.DaybreakGames.Live.CensusStream;
using Voidwell.DaybreakGames.Live.CensusStream.JsonConverters;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Live.Test;

public class StreamJsonTest
{
    private record BoolHolder(bool Value);
    private record DateHolder(DateTime Value);

    private static readonly JsonSerializerOptions ConverterOptions = new()
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
        JsonSerializer.Deserialize<bool>(json, ConverterOptions).Should().Be(expected);
    }

    [Fact]
    public void BooleanJsonConverter_WritesJsonBoolean()
    {
        JsonSerializer.Serialize(true, ConverterOptions).Should().Be("true");
        JsonSerializer.Serialize(false, ConverterOptions).Should().Be("false");
    }

    [Fact]
    public void DateTimeJsonConverter_ReadsEpochSecondsAsUtc()
    {
        var result = JsonSerializer.Deserialize<DateTime>("\"1700000000\"", ConverterOptions);

        result.Should().Be(new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc));
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void DateTimeJsonConverter_ReadsIsoStringsAsUtc()
    {
        var result = JsonSerializer.Deserialize<DateTime>("\"2024-03-05T10:20:30\"", ConverterOptions);

        result.Should().Be(new DateTime(2024, 3, 5, 10, 20, 30, DateTimeKind.Utc));
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void DateTimeJsonConverter_WritesUtcIsoString()
    {
        var json = JsonSerializer.Serialize(new DateTime(2024, 3, 5, 10, 20, 30, DateTimeKind.Utc), ConverterOptions);

        json.Should().Be("\"2024-03-05T10:20:30Z\"");
    }

    [Theory]
    [InlineData("EventName", "event_name")]
    [InlineData("WorldId", "world_id")]
    [InlineData("CharacterId", "character_id")]
    [InlineData("Timestamp", "timestamp")]
    [InlineData("ZoneId", "zone_id")]
    public void UnderscorePropertyJsonNamingPolicy_ConvertsPascalCaseToSnakeCase(string name, string expected)
    {
        new UnderscorePropertyJsonNamingPolicy().ConvertName(name).Should().Be(expected);
    }

    [Fact]
    public void UnderscorePropertyJsonNamingPolicy_NullName_Throws()
    {
        var act = () => new UnderscorePropertyJsonNamingPolicy().ConvertName(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void SerializerOptions_DeserializesCensusPayloadWithStringEncodedValues()
    {
        const string json = """
            {"character_id":"5428","event_name":"PlayerLogin","timestamp":"1700000000","world_id":"17","zone_id":"2"}
            """;

        var payload = JsonSerializer.Deserialize<PlayerLogin>(json, StreamConstants.SerializerOptions);

        payload.CharacterId.Should().Be("5428");
        payload.EventName.Should().Be("PlayerLogin");
        payload.WorldId.Should().Be(17);
        payload.ZoneId.Should().Be(2);
        payload.Timestamp.Should().Be(new DateTime(2023, 11, 14, 22, 13, 20, DateTimeKind.Utc));
    }

    [Fact]
    public void SerializerOptions_PayloadWithoutZone_HasNullZoneId()
    {
        const string json = """{"event_name":"PlayerLogin","timestamp":"1700000000","world_id":"17"}""";

        JsonSerializer.Deserialize<PayloadBase>(json, StreamConstants.SerializerOptions).ZoneId.Should().BeNull();
    }
}
