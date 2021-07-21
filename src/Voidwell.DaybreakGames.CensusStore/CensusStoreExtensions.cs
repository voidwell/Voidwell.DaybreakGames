using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.CensusStore.StoreUpdater;

namespace Voidwell.DaybreakGames.CensusStore
{
    public static class CensusStoreExtensions
    {
        public static IServiceCollection AddCensusStores(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<StoreOptions>(configuration);

            services.AddCensusHelpers();

            services.AddSingleton<ICharacterStore, CharacterStore>();
            services.AddSingleton<IOutfitStore, OutfitStore>();
            services.AddSingleton<IItemStore, ItemStore>();
            services.AddSingleton<IVehicleStore, VehicleStore>();
            services.AddSingleton<IFactionStore, FactionStore>();
            services.AddSingleton<ITitleStore, TitleStore>();
            services.AddSingleton<IProfileStore, ProfileStore>();
            services.AddSingleton<IWorldStore, WorldStore>();
            services.AddSingleton<IZoneStore, ZoneStore>();
            services.AddSingleton<IMapStore, MapStore>();
            services.AddSingleton<IExperienceStore, ExperienceStore>();
            services.AddSingleton<IMetagameEventStore, MetagameEventStore>();

            services.AddSingleton<IStoreUpdaterService, StoreUpdaterService>();
            services.AddHostedService<StoreUpdaterSchedulerHostedService>();

            return services;
        }
    }
}
