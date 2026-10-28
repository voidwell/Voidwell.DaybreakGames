using System.Text.Json;

namespace Voidwell.DaybreakGames.Live.CensusStream;

internal static class JsonElementExtensions
{
    public static string? TryGetPropertyString(this JsonElement json, string name)
    {
        if (json.ValueKind != JsonValueKind.Object || !json.TryGetProperty(name, out var value))
        {
            return null;
        }

        return value.ToString();
    }
}
