using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusStream;
using Voidwell.DaybreakGames.HostedServices;
using Voidwell.DaybreakGames.HttpAuthenticatedClient;
using Voidwell.DaybreakGames.Messages;
using Voidwell.DaybreakGames.Services;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.App
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<DaybreakGamesOptions>(configuration);
            services.Configure<DaybreakGamesOptions>(options =>
            {
                var eventNames = configuration.GetValue<string>("CensusWebsocketServices");
                var experienceIds = configuration.GetValue<string>("CensusWebsocketExperienceIds");

                options.CensusWebsocketServices = eventNames?.Replace(" ", "").Split(",");

                if (experienceIds != null)
                {
                    options.CensusWebsocketExperienceIds = experienceIds.Replace(" ", "").Split(",");
                }
            });

            services.AddAuthenticatedHttpClient(options =>
            {
                options.TokenServiceAddress = "http://voidwellauth:5000/connect/token";
                options.ClientId = configuration.GetValue<string>("ClientId");
                options.ClientSecret = configuration.GetValue<string>("ClientSecret");
                options.Scopes = new List<string>
                    {
                        "voidwell-messagewell-publish"
                    };
            });

            services.AddCensusServices(options =>
            {
                options.CensusServiceId = configuration.GetValue<string>("CensusServiceKey");
                options.CensusServiceNamespace = configuration.GetValue<string>("CensusServiceNamespace");
            });

            services.AddCensusHelpers();
            services.AddUpdateableTasks();

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

            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<ICharacterService, CharacterService>();
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
            services.AddSingleton<IWebsocketEventHandler, WebsocketEventHandler>();
            services.AddSingleton<IWebsocketMonitor, WebsocketMonitor>();

            services.AddHostedService<StoreUpdaterSchedulerHostedService>();
            services.AddHostedService<WebsocketMonitorHostedService>();
            services.AddHostedService<CharacterUpdaterHostedService>();

            return services;
        }
    }
}
