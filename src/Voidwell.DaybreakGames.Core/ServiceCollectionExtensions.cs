using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.Core.Services;
using Voidwell.DaybreakGames.Core.Services.Planetside;

namespace Voidwell.DaybreakGames.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureCoreServices(this IServiceCollection services)
        {
            services.AddTransient<IFeedService, FeedService>();
            services.AddTransient<IItemService, ItemService>();
            services.AddTransient<ITitleService, TitleService>();
            services.AddTransient<IVehicleService, VehicleService>();
            services.AddTransient<IZoneService, ZoneService>();
            services.AddTransient<IWeaponService, WeaponService>();
            services.AddTransient<IAlertService, AlertService>();
            services.AddTransient<ICombatReportService, CombatReportService>();
            services.AddTransient<IMetagameEventService, MetagameEventService>();
            services.AddTransient<IFactionService, FactionService>();
            services.AddTransient<ISearchService, SearchService>();
            services.AddTransient<IGradeService, GradeService>();
            services.AddTransient<IExperienceService, ExperienceService>();

            services.AddSingleton<ICharacterService, CharacterService>();
            services.AddSingleton<ICharacterSessionService, CharacterSessionService>();
            services.AddSingleton<IOutfitService, OutfitService>();
            services.AddSingleton<IWorldService, WorldService>();
            services.AddSingleton<IWeaponAggregateService, WeaponAggregateService>();
            services.AddSingleton<IPSBUtilityService, PSBUtilityService>();
            services.AddSingleton<ICharacterRatingService, CharacterRatingService>();
            services.AddSingleton<IMapService, MapService>();
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddSingleton<IWorldEventsService, WorldEventsService>();
            services.AddSingleton<ICharacterUpdaterService, CharacterUpdaterService>();
        }
    }
}
