using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Voidwell.DaybreakGames.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services, Action<CacheOptions> actionOptions)
    {
        services.AddOptions();
        services.Configure(actionOptions);

        services.TryAddSingleton<ICacheConnector, CacheConnector>();
        services.TryAddSingleton<ICache, Cache>();

        return services;
    }
}
