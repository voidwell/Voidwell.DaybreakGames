using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Voidwell.Cache
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration, string keyPrefix)
        {
            services.AddOptions();
            services.AddSingleton(impl => impl.GetRequiredService<IOptions<CacheOptions>>().Value);
            services.Configure<CacheOptions>(configuration);
            services.Configure<CacheOptions>(a => a.KeyPrefix = keyPrefix);

            services.AddSingleton<ICache, Cache>();

            return services;
        }
    }
}
