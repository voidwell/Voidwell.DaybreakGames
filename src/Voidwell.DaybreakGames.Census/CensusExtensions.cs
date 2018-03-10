using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Voidwell.DaybreakGames.Census
{
    public static class CensusExtensions
    {
        public static IServiceCollection AddCensusClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<CensusOptions>(configuration);

            services.AddSingleton<ICensusClient, CensusClient>();

            return services;
        }
    }
}
