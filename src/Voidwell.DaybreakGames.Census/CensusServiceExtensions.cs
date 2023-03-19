using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Census.Patcher;

namespace Voidwell.DaybreakGames.Census
{
    public static class CensusServiceExtensions
    {
        public static IServiceCollection AddCensusCollections(this IServiceCollection services)
        {
            services.TryAddSingleton<ICensusPatchClient, SanctuaryCensusClient>();

            services.TryAddTransient<AchievementCollection>();
            services.TryAddTransient<CharactersAchievementCollection>();
            services.TryAddTransient<CharacterCollection>();
            services.TryAddTransient<CharacterNameCollection>();
            services.TryAddTransient<CharactersDirectiveCollection>();
            services.TryAddTransient<CharactersDirectiveObjectiveCollection>();
            services.TryAddTransient<CharactersDirectiveTierCollection>();
            services.TryAddTransient<CharactersDirectiveTreeCollection>();
            services.TryAddTransient<CharactersStatCollection>();
            services.TryAddTransient<CharactersStatByFactionCollection>();
            services.TryAddTransient<CharactersStatHistoryCollection>();
            services.TryAddTransient<CharactersWeaponStatCollection>();
            services.TryAddTransient<CharactersWeaponStatByFactionCollection>();
            services.TryAddTransient<DirectiveCollection>();
            services.TryAddTransient<DirectiveTierCollection>();
            services.TryAddTransient<DirectiveTreeCategoryCollection>();
            services.TryAddTransient<DirectiveTreeCollection>();
            services.TryAddTransient<ExperienceCollection>();
            services.TryAddTransient<FacilityLinkCollection>();
            services.TryAddTransient<FactionCollection>();
            services.TryAddTransient<ImageSetCollection>();
            services.TryAddTransient<ItemCollection>();
            services.TryAddTransient<ItemCategoryCollection>();
            services.TryAddTransient<LoadoutCollection>();
            services.TryAddTransient<MapCollection>();
            services.TryAddTransient<MapHexCollection>();
            services.TryAddTransient<MapRegionCollection>();
            services.TryAddTransient<MetagameEventCollection>();
            services.TryAddTransient<MetagameEventStateCollection>();
            services.TryAddTransient<ObjectiveCollection>();
            services.TryAddTransient<ObjectiveSetToObjectiveCollection>();
            services.TryAddTransient<OutfitCollection>();
            services.TryAddTransient<OutfitMembershipCollection>();
            services.TryAddTransient<ProfileCollection>();
            services.TryAddTransient<RewardCollection>();
            services.TryAddTransient<RewardGroupToRewardCollection>();
            services.TryAddTransient<RewardSetToRewardGroupCollection>();
            services.TryAddTransient<TitleCollection>();
            services.TryAddTransient<VehicleCollection>();
            services.TryAddTransient<VehicleFactionCollection>();
            services.TryAddTransient<WorldCollection>();
            services.TryAddTransient<WorldEventCollection>();
            services.TryAddTransient<ZoneCollection>();

            return services;
        }
    }
}
