using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voidwell.DaybreakGames.Services.Mappers;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(
                typeof(CensusToDomainMappingProfile).Assembly,
                typeof(DataToDomainMappingProfile).Assembly,
                typeof(DomainToDomainMappingProfile).Assembly);

            services.TryAddTransient<IItemService, ItemService>();
            services.TryAddTransient<IVehicleService, VehicleService>();
            services.TryAddTransient<IWeaponService, WeaponService>();
            services.TryAddTransient<IAlertService, AlertService>();
            services.TryAddTransient<ICombatReportService, CombatReportService>();
            services.TryAddTransient<IMetagameEventService, MetagameEventService>();
            services.TryAddTransient<ISearchService, SearchService>();
            services.TryAddTransient<IGradeService, GradeService>();
            services.TryAddTransient<IFeedService, FeedService>();

            services.TryAddSingleton<ICharacterService, CharacterService>();
            services.TryAddSingleton<ILeaderboardService, LeaderboardService>();
            services.TryAddSingleton<ICharacterSessionService, CharacterSessionService>();
            services.TryAddSingleton<IOutfitService, OutfitService>();
            services.TryAddSingleton<IWorldService, WorldService>();
            services.TryAddSingleton<IWeaponAggregateService, WeaponAggregateService>();
            services.TryAddSingleton<ICharacterRatingService, CharacterRatingService>();
            services.TryAddSingleton<IMapService, MapService>();
            services.TryAddSingleton<IProfileService, ProfileService>();
            services.TryAddSingleton<IWorldEventsService, WorldEventsService>();
            services.TryAddSingleton<IPSBUtilityService, PSBUtilityService>();

            return services;
        }
    }
}
