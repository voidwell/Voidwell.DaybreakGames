using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.CensusStore.StoreUpdater;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Voidwell.DaybreakGames.CensusStore
{
    public static class CensusStoreExtensions
    {
        public static IServiceCollection AddCensusStores(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<StoreOptions>(configuration);

            services.AddAutoMapper(typeof(CensusToEntityMappingProfile).Assembly);

            services.AddCensusCollections();

            services.AddSingleton<ICharacterStore, CharacterStore>();
            services.AddSingleton<IExperienceStore, ExperienceStore>();
            services.AddSingleton<IFacilityLinkStore, FacilityLinkStore>();
            services.AddSingleton<IFactionStore, FactionStore>();
            services.AddSingleton<IItemCategoryStore, ItemCategoryStore>();
            services.AddSingleton<IItemStore, ItemStore>();
            services.AddSingleton<ILoadoutStore, LoadoutStore>();
            services.AddSingleton<IMapStore, MapStore>();
            services.AddSingleton<IMapHexStore, MapHexStore>();
            services.AddSingleton<IMapRegionStore, MapRegionStore>();
            services.AddSingleton<IMetagameEventStateStore, MetagameEventStateStore>();
            services.AddSingleton<IMetagameEventStore, MetagameEventStore>();
            services.AddSingleton<IOutfitStore, OutfitStore>();
            services.AddSingleton<IProfileStore, ProfileStore>();
            services.AddSingleton<ITitleStore, TitleStore>();
            services.AddSingleton<IVehicleFactionStore, VehicleFactionStore>();
            services.AddSingleton<IVehicleStore, VehicleStore>();
            services.AddSingleton<IWorldStore, WorldStore>();
            services.AddSingleton<IZoneStore, ZoneStore>();

            services.TryAddSingleton<IStoreUpdaterService, StoreUpdaterService>();
            services.AddHostedService<StoreUpdaterSchedulerHostedService>();

            return services;
        }
    }
}
