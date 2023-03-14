using System.Text.Json;
using Voidwell.DaybreakGames.Live.CensusStream.JsonConverters;

namespace Voidwell.DaybreakGames.Live.CensusStream
{
    public static class StreamConstants
    {
        public static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new UnderscorePropertyJsonNamingPolicy(),
            Converters =
            {
                new BooleanJsonConverter(),
                new DateTimeJsonConverter()
            }
        };
    }
}
