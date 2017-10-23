using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Voidwell.DaybreakGames.Census
{
    public static class CensusExtensions
    {
        public static IServiceCollection AddCensusClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.AddSingleton(impl => impl.GetRequiredService<IOptions<CensusOptions>>().Value);
            services.Configure<CensusOptions>(configuration);

            services.AddTransient<ICensusClient, CensusClient>();

            return services;
        }
    }
}
