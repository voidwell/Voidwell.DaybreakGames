using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.App.HostedServices;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusStream;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.GameState;

namespace Voidwell.DaybreakGames.App
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<DaybreakGamesOptions>(configuration);
            services.Configure<DaybreakGamesOptions>(options =>
            {
                var eventNames = configuration.GetValue<string>("CensusWebsocketServices");
                var experienceIds = configuration.GetValue<string>("CensusWebsocketExperienceIds");

                options.CensusWebsocketServices = eventNames?.Replace(" ", "").Split(",");

                if (experienceIds != null)
                {
                    options.CensusWebsocketExperienceIds = experienceIds.Replace(" ", "").Split(",");
                }
            });

            services.AddCensusServices(options =>
            {
                options.CensusServiceId = configuration.GetValue<string>("CensusServiceKey");
                options.CensusServiceNamespace = configuration.GetValue<string>("CensusServiceNamespace");
            });

            services.AddCensusHelpers();
            services.AddDaybreakGamesServices();
            services.AddCensusStreamServices();
            services.AddGameStateServices();

            services.AddHostedService<StoreUpdaterSchedulerHostedService>();
            services.AddHostedService<WebsocketMonitorHostedService>();
            services.AddHostedService<CharacterUpdaterHostedService>();


            return services;
        }
    }
}
