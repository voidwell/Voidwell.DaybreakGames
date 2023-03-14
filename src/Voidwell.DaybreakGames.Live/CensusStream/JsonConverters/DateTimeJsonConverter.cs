using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Live.CensusStream.JsonConverters
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (double.TryParse(reader.GetString(), out var epochTimestamp))
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epochTimestamp);
            }

            return DateTime.SpecifyKind(DateTime.Parse(reader.GetString()), DateTimeKind.Utc);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToUniversalTime());
    }
}
