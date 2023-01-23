using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;
using Voidwell.DaybreakGames.CensusStore;
using Voidwell.DaybreakGames.Live.CensusStream;
using Voidwell.DaybreakGames.Live.GameState;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voidwell.DaybreakGames.Live.Mappers;

namespace Voidwell.DaybreakGames.Live
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLiveServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(typeof(CensusToDataMappingProfile).Assembly);

            services.AddOptions();
            services.Configure<LiveOptions>(configuration);
            services.Configure<LiveOptions>(options =>
            {
                var eventNames = configuration.GetValue<string>("CensusWebsocketServices");
                var experienceIds = configuration.GetValue<string>("CensusWebsocketExperienceIds");

                options.CensusWebsocketServices = eventNames?.Replace(" ", "").Split(",");

                if (experienceIds != null)
                {
                    options.CensusWebsocketExperienceIds = experienceIds.Replace(" ", "").Split(",");
                }
            });

            services.AddCensusStores(configuration);
            services.AddEventProcessors();

            services.TryAddSingleton<IPlayerMonitor, PlayerMonitor>();
            services.TryAddSingleton<IWorldMonitor, WorldMonitor>();

            services.AddSingleton<ICharacterUpdaterService, CharacterUpdaterService>();
            services.AddSingleton<IWebsocketEventHandler, WebsocketEventHandler>();
            services.AddSingleton<IWebsocketMonitor, WebsocketMonitor>();
            services.AddSingleton<IWebsocketHealthMonitor, WebsocketHealthMonitor>();
            services.AddSingleton<IEventValidator, EventValidator>();
            services.AddSingleton<IEventProcessorHandler, EventProcessorHandler>();

            services.AddHostedService<WebsocketMonitorHostedService>();
            services.AddHostedService<CharacterUpdaterHostedService>();

            return services;
        }

        private static void AddEventProcessors(this IServiceCollection services)
        {
            typeof(IEventProcessor<>).GetTypeInfo().Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => t.GetInterfaces().Select(i => (t, i)))
                .Where(a => a.i.IsGenericType && typeof(IEventProcessor<>).IsAssignableFrom(a.i.GetGenericTypeDefinition()))
                .ToList()
                .ForEach(a => services.AddSingleton(a.i, a.t));
        }
    }
}
