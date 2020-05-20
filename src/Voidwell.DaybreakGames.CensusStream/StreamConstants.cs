using DaybreakGames.Census;
using DaybreakGames.Census.JsonConverters;
using Newtonsoft.Json;

namespace Voidwell.DaybreakGames.CensusStream
{
    public static class StreamConstants
    {
        public static readonly JsonSerializer PayloadDeserializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = new UnderscorePropertyNamesContractResolver(),
            Converters = new JsonConverter[]
                {
                    new BooleanJsonConverter(),
                    new DateTimeJsonConverter()
                }
        });
    }
}
