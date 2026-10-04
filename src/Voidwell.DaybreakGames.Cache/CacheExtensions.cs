using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace Voidwell.DaybreakGames.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services, Action<CacheOptions> actionOptions)
    {
        var cacheOptions = new CacheOptions();
        actionOptions(cacheOptions);

        services.AddSingleton(Options.Create(cacheOptions));

        var cacheBuilder = services
            .AddFusionCache()
            .WithOptions(options =>
            {
                options.CacheKeyPrefix = cacheOptions.KeyPrefix ?? Assembly.GetExecutingAssembly().GetName().Name;
                options.DefaultEntryOptions.SkipBackplaneNotifications = true;

                // A Redis or serialization problem should degrade to the in-memory cache, not fail the request
                options.DefaultEntryOptions.ReThrowSerializationExceptions = false;
                options.DefaultEntryOptions.ReThrowDistributedCacheExceptions = false;
            });

        if (!string.IsNullOrWhiteSpace(cacheOptions.RedisConfiguration))
        {
            // Redis is the shared L2 cache; the in-memory L1 cache sits in front of it
#pragma warning disable CA2000 // RedisCache ownership passes to FusionCache and the service provider
            cacheBuilder
                .WithSerializer(new FusionCacheSystemTextJsonSerializer(new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                }))
                .WithDistributedCache(new RedisCache(new RedisCacheOptions
                {
                    Configuration = cacheOptions.RedisConfiguration
                }));
#pragma warning restore CA2000

            cacheBuilder.WithStackExchangeRedisBackplane(options =>
            {
                options.Configuration = cacheOptions.RedisConfiguration;
            });

            services.TryAddSingleton<IListStore, RedisListStore>();
        }
        else
        {
            services.TryAddSingleton<IListStore, MemoryListStore>();
        }

        services.TryAddSingleton<ICache, Cache>();

        return services;
    }

}
