using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Voidwell.DaybreakGames.Live.CensusStream;
using Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Utils.HostedService;

namespace Voidwell.DaybreakGames.Live;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiveServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<LiveOptions>(configuration);
        services.Configure<LiveOptions>(options =>
        {
            var eventNames = configuration.GetValue<string>("CensusWebsocketServices");

            options.CensusWebsocketServices = eventNames?.Replace(" ", "").Split(",");
        });

        services.AddEventProcessors();

        services.TryAddSingleton<IPlayerMonitor, PlayerMonitor>();
        services.TryAddSingleton<IWorldMonitor, WorldMonitor>();

        services.AddSingleton<IWebsocketEventHandler, WebsocketEventHandler>();
        services.AddSingleton<IEventValidator, EventValidator>();
        services.AddSingleton<IEventProcessorHandler, EventProcessorHandler>();

        services.AddStatefulHostedService<WebsocketMonitor>();
        services.AddStatefulHostedService<ICharacterUpdaterService, CharacterUpdaterService>();

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
