using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.CensusStore.StoreUpdater;
using Voidwell.DaybreakGames.Utils.HostedService;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;

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

            services.AddSingleton<IStoreUpdaterHelper, StoreUpdaterHelper>();

            services.AddSingleton<ICharacterStore, CharacterStore>();
            services.AddSingleton<ICharacterDirectiveStore, CharacterDirectiveStore>();
            services.AddSingleton<IFacilityLinkStore, FacilityLinkStore>();
            services.AddSingleton<IItemStore, ItemStore>();
            services.AddSingleton<ILoadoutStore, LoadoutStore>();
            services.AddSingleton<IMapStore, MapStore>();
            services.AddSingleton<IMapHexStore, MapHexStore>();
            services.AddSingleton<IMapRegionStore, MapRegionStore>();
            services.AddSingleton<IMetagameEventStore, MetagameEventStore>();
            services.AddSingleton<IOutfitStore, OutfitStore>();
            services.AddSingleton<IProfileStore, ProfileStore>();
            services.AddSingleton<IVehicleStore, VehicleStore>();
            services.AddSingleton<IWorldStore, WorldStore>();
            services.AddSingleton<IZoneStore, ZoneStore>();

            services.AddStatefulHostedService<IStoreUpdaterService, StoreUpdaterScheduler>();

            services.Configure<StaticStoreUpdaterConfiguration>(o => o.Enabled = !configuration.GetValue<bool>("DisableUpdater"));

            services.AddUpdaterStaticStore<AchievementCollection, Achievement>();
            services.AddUpdaterStaticStore<DirectiveCollection, Directive>()
                .WithUpdateDependency<DirectiveTierCollection>();
            services.AddUpdaterStaticStore<DirectiveTreeCategoryCollection, DirectiveTreeCategory>();
            services.AddUpdaterStaticStore<DirectiveTreeCollection, DirectiveTree>()
                .WithUpdateDependency<DirectiveTreeCategoryCollection>();
            services.AddUpdaterStaticStore<DirectiveTierCollection, DirectiveTier>()
                .WithUpdateDependency<DirectiveTreeCollection>();
            services.AddUpdaterStaticStore<ExperienceCollection, Experience>();
            services.AddUpdaterStaticStore<FacilityLinkCollection, FacilityLink>();
            services.AddUpdaterStaticStore<FactionCollection, Faction>();
            services.AddUpdaterStaticStore<ImageSetCollection, ImageSet>();
            services.AddUpdaterStaticStore<ItemCategoryCollection, ItemCategory>();
            services.AddUpdaterStaticStore<ItemCollection, Item>();
            services.AddUpdaterStaticStore<LoadoutCollection, Loadout>();
            services.AddUpdaterStaticStore<MapHexCollection, MapHex>();
            services.AddUpdaterStaticStore<MapRegionCollection, MapRegion>();
            services.AddUpdaterStaticStore<MetagameEventStateCollection, MetagameEventState>();
            services.AddUpdaterStaticStore<MetagameEventCollection, MetagameEventCategory>();
            services.AddUpdaterStaticStore<ObjectiveCollection, Objective>()
                .WithUpdateDependency<ObjectiveSetToObjectiveCollection>();
            services.AddUpdaterStaticStore<ObjectiveSetToObjectiveCollection, ObjectiveSetToObjective>();
            services.AddUpdaterStaticStore<ProfileCollection, Profile>();
            services.AddUpdaterStaticStore<RewardCollection, Reward>()
                .WithUpdateDependency<RewardGroupToRewardCollection>();
            services.AddUpdaterStaticStore<RewardGroupToRewardCollection, RewardGroupToReward>()
                .WithUpdateDependency<RewardSetToRewardGroupCollection>();
            services.AddUpdaterStaticStore<RewardSetToRewardGroupCollection, RewardSetToRewardGroup>();

            return services;
        }
    }
}
