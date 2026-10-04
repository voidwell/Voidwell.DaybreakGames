using ZiggyCreatures.Caching.Fusion;

namespace Voidwell.DaybreakGames.Test.Caching;

/// <summary>
/// A real <see cref="Voidwell.DaybreakGames.Cache.ICache"/> backed by an in-memory FusionCache, for testing caching behavior.
/// </summary>
public static class TestCache
{
    public static Voidwell.DaybreakGames.Cache.ICache Create()
    {
#pragma warning disable CA2000 // The cache lives for the duration of the test, so it is intentionally not disposed
        return new Voidwell.DaybreakGames.Cache.Cache(
            new FusionCache(new FusionCacheOptions()),
            new Voidwell.DaybreakGames.Cache.MemoryListStore());
#pragma warning restore CA2000
    }
}
