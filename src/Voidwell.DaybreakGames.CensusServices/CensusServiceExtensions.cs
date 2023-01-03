using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.CensusServices.Patcher;
using Voidwell.DaybreakGames.CensusServices.Patcher.Services;

namespace Voidwell.DaybreakGames.CensusServices
{
    public static class CensusServiceExtensions
    {
        public static IServiceCollection AddCensusHelpers(this IServiceCollection services)
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
            services.AddSingleton<CensusExperience>();
            services.AddSingleton<CensusWorldEvent>();
            services.AddSingleton<CensusLoadout>();

            services.AddSingleton<ICensusItem, PatchItem>();
            services.AddSingleton<ICensusItemCategory, PatchItemCategory>();
            services.AddSingleton<ICensusMap, PatchMap>();
            services.AddSingleton<ICensusWorld, PatchWorld>();
            services.AddSingleton<ICensusZone, PatchZone>();

            services.AddSingleton<IPatchClient, SanctuaryCensusClient>();

            return services;
        }
    }
}
