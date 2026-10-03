using ZiggyCreatures.Caching.Fusion;

namespace Voidwell.DaybreakGames.Test.Caching;

/// <summary>
/// A real <see cref="Voidwell.DaybreakGames.Cache.ICache"/> backed by an in-memory FusionCache, for testing caching behavior.
/// </summary>
public static class TestCache
{
    public static Voidwell.DaybreakGames.Cache.ICache Create()
    {
        return new Voidwell.DaybreakGames.Cache.Cache(
            new FusionCache(new FusionCacheOptions()),
            new Voidwell.DaybreakGames.Cache.MemoryListStore());
    }
}
