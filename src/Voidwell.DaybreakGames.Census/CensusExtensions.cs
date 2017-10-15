using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Voidwell.DaybreakGames.Census
{
    public static class CensusExtensions
    {
        public static IServiceCollection AddCensusClient(this IServiceCollection services, IConfiguration censusConfig)
        {
            services.AddOptions();
            services.AddSingleton(impl => impl.GetRequiredService<IOptions<CensusOptions>>().Value);
            services.Configure<CensusOptions>(censusConfig);

            services.AddTransient<ICensusClient, CensusClient>();

            return services;
        }
    }
}
