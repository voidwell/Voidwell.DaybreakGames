using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Census.Platforms;

public static class PlatformExtensions
{
    public static string GetCensusNamespace(this Platform platform)
    {
        return platform switch
        {
            Platform.PC => "ps2",
            Platform.PS4US => "ps2ps4us",
            Platform.PS4EU => "ps2ps4eu",
            _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, null)
        };
    }

    /// <summary>
    /// The key the platform's Census services are registered under.
    /// </summary>
    public static string GetCensusServiceKey(this Platform platform)
    {
        return platform.ToString();
    }
}
