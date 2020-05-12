using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.Census.Services;

namespace Voidwell.DaybreakGames.Census
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureCensusServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<DaybreakGamesOptions>(configuration);
            services.Configure<DaybreakGamesOptions>(options =>
            {
                var eventNames = configuration.GetValue<string>("CensusWebsocketServices");
                var experienceIds = configuration.GetValue<string>("CensusWebsocketExperienceIds");

                options.CensusWebsocketServices = eventNames?.Replace(" ", "").Split(',');

                if (experienceIds != null)
                {
                    options.CensusWebsocketExperienceIds = experienceIds.Replace(" ", "").Split(',');
                }
            });

            services.AddCensusServices(options =>
            {
                options.CensusServiceId = configuration.GetValue<string>("CensusServiceKey");
                options.CensusServiceNamespace = configuration.GetValue<string>("CensusServiceNamespace");
            });

            services.AddSingleton<CensusCharacter>();
            services.AddSingleton<CensusFaction>();
            services.AddSingleton<CensusItem>();
            services.AddSingleton<CensusItemCategory>();
            services.AddSingleton<CensusMap>();
            services.AddSingleton<CensusMetagameEvent>();
            services.AddSingleton<CensusOutfit>();
            services.AddSingleton<CensusProfile>();
            services.AddSingleton<CensusTitle>();
            services.AddSingleton<CensusVehicle>();
            services.AddSingleton<CensusWorld>();
            services.AddSingleton<CensusZone>();
            services.AddSingleton<CensusExperience>();
            services.AddSingleton<CensusWorldEvent>();
            services.AddSingleton<CensusLoadout>();
        }
    }
}
