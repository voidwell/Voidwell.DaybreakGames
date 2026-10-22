using System.Globalization;

namespace Voidwell.DaybreakGames.Census.Models.Extensions;

// Census returns character and outfit ids as strings; they are stored as bigint.
public static class CensusIdExtensions
{
    public static long ToCensusId(this string? value)
    {
        return value.ToOptionalCensusId() ?? throw new FormatException($"'{value}' is not a valid Census id.");
    }

    public static long? ToOptionalCensusId(this string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        return long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var id)
            ? id
            : throw new FormatException($"'{value}' is not a valid Census id.");
    }
}
