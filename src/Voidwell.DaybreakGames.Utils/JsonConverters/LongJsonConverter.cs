using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Voidwell.DaybreakGames.Utils.JsonConverters;

public sealed class LongJsonConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(long) || typeToConvert == typeof(long?);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return typeToConvert == typeof(long)
            ? new LongConverter()
            : new NullableLongConverter();
    }

    private sealed class LongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && long.TryParse(reader.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out long value))
            {
                return value;
            }

            return reader.GetInt64();
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
        }
    }

    private sealed class NullableLongConverter : JsonConverter<long?>
    {
        public override bool HandleNull => true;

        public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String && long.TryParse(reader.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out long value))
            {
                return value;
            }

            return reader.GetInt64();
        }

        public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
