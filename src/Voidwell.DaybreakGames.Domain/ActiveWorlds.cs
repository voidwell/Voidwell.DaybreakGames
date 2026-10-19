namespace Voidwell.DaybreakGames.Domain;

/// <summary>
/// The worlds (servers) currently live. The database also holds legacy worlds that have since
/// been merged or shut down; endpoints that should only show current servers filter on this list.
/// </summary>
public static class ActiveWorlds
{
    public const int Osprey = 1;
    public const int Wainwright = 10;
    public const int Jaeger = 19;
    public const int Soltech = 40;

    public static readonly IReadOnlySet<int> Ids = new HashSet<int> { Osprey, Wainwright, Jaeger, Soltech };

    public static bool Contains(int worldId) => Ids.Contains(worldId);
}
