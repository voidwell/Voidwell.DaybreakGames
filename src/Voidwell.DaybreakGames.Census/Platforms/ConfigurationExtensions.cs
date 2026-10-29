using Microsoft.Extensions.Configuration;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Census.Platforms;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Reads the comma-separated <c>CensusPlatforms</c> setting. Every platform is enabled when it is empty.
    /// </summary>
    public static IEnumerable<Platform> GetCensusPlatforms(this IConfiguration configuration)
    {
        var value = configuration.GetValue<string>("CensusPlatforms");
        if (string.IsNullOrWhiteSpace(value))
        {
            return Enum.GetValues<Platform>();
        }

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(name => Enum.TryParse<Platform>(name, true, out var platform)
                ? platform
                : throw new InvalidOperationException($"Unknown platform '{name}' in CensusPlatforms. Valid values: {string.Join(", ", Enum.GetNames<Platform>())}."))
            .Distinct()
            .ToList();
    }
}
