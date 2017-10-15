using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Voidwell.Cache
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services, string keyPrefix)
        {
            services.AddOptions();
            services.AddSingleton(impl => impl.GetRequiredService<IOptions<CacheOptions>>().Value);
            services.Configure<CacheOptions>(options => options.KeyPrefix = keyPrefix);

            services.AddSingleton<ICache, Cache>();

            return services;
        }
    }
}
