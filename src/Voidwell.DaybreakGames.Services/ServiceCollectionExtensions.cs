using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Voidwell.DaybreakGames.Services;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.App
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDaybreakGamesServices(this IServiceCollection services)
        {
            services.AddUpdateableServices();

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
            services.AddSingleton<IWorldMonitor, WorldMonitor>();
            services.AddSingleton<IPlayerMonitor, PlayerMonitor>();
            services.AddSingleton<IWorldService, WorldService>();
            services.AddSingleton<IWeaponAggregateService, WeaponAggregateService>();
            services.AddSingleton<IPSBUtilityService, PSBUtilityService>();
            services.AddSingleton<ICharacterRatingService, CharacterRatingService>();
            services.AddSingleton<IMapService, MapService>();
            services.AddSingleton<IProfileService, ProfileService>();
            services.AddSingleton<IWorldEventsService, WorldEventsService>();

            services.AddSingleton<ICharacterUpdaterService, CharacterUpdaterService>();

            return services;
        }

        private static void AddUpdateableServices(this IServiceCollection services)
        {
            typeof(IUpdateable).GetTypeInfo().Assembly.GetTypes()
                .Where(a => typeof(IUpdateable).IsAssignableFrom(a) && a.GetTypeInfo().IsClass && !a.GetTypeInfo().IsAbstract)
                .ToList()
                .ForEach(t => services.AddTransient(typeof(IUpdateable), t));
        }
    }
}
