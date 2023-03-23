using System.Text.Json;
using System.Text.Json.Serialization;
using Voidwell.DaybreakGames.Live.CensusStream.JsonConverters;

namespace Voidwell.DaybreakGames.Live.CensusStream
{
    public static class StreamConstants
    {
        public static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new UnderscorePropertyJsonNamingPolicy(),
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters =
            {
                new BooleanJsonConverter(),
                new DateTimeJsonConverter()
            }
        };
    }
}
