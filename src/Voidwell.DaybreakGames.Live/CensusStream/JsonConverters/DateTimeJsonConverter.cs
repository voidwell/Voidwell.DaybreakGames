using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Live.CensusStream.JsonConverters;

/// <summary>
/// Reads Census stream dates (epoch seconds or date strings) and writes dates, as UTC whatever the server's time zone.
/// A date string without a zone is taken to be UTC.
/// </summary>
public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()!;

        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var epochTimestamp))
        {
            return DateTime.UnixEpoch.AddSeconds(epochTimestamp);
        }

        return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        });
    }
}
