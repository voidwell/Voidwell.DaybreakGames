using Microsoft.Extensions.DependencyInjection;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusServiceExtensions
    {
        public static IServiceCollection AddCensusServices(this IServiceCollection services)
        {
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

            return services;
        }
    }
}
