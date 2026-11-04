using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Utils.JsonConverters;

/// <summary>
/// Reads and writes dates as UTC, whatever the server's time zone. Dates without a zone are taken to be UTC:
/// that is how they are stored (`timestamp without time zone` columns come back with an unspecified kind).
/// </summary>
public sealed class DateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString()!, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
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
