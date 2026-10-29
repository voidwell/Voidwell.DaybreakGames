using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Census.Platforms;

public class CensusPlatformOptions
{
    public string CensusServiceId { get; set; } = null!;
    public string? UserAgent { get; set; }

    /// <summary>
    /// Platforms whose worlds are tracked: streamed, updated and searched. Census clients exist for every platform regardless.
    /// </summary>
    public IEnumerable<Platform> Platforms { get; set; } = Enum.GetValues<Platform>();

    /// <summary>
    /// Platform used when none is set for the current flow. Static collections are identical across platforms, so they rely on it.
    /// </summary>
    public Platform DefaultPlatform { get; set; } = Platform.PC;
}
