using Microsoft.Extensions.DependencyInjection;
using Voidwell.Common.Cache;

namespace Voidwell.DaybreakGames.Test.Caching;

/// <summary>
/// A real <see cref="ICache"/> backed by an in-memory FusionCache (no Redis), for testing caching behavior.
/// </summary>
public static class TestCache
{
    public static ICache Create()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCache(_ => { });

        return services.BuildServiceProvider().GetRequiredService<ICache>();
    }
}
