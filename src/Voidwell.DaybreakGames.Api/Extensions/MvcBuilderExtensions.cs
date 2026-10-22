using System.Text.Json;
using System.Text.Json.Serialization;
using Voidwell.DaybreakGames.Utils.JsonConverters;

namespace Voidwell.DaybreakGames.Api.Extensions;

internal static class MvcBuilderExtensions
{
    public static IMvcBuilder AddApiJsonOptions(this IMvcBuilder builder)
    {
        return builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
            options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
        });
    }
}
