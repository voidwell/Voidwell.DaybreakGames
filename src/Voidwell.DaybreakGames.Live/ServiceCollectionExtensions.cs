using System.Reflection;
using DaybreakGames.Census.Stream;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.Live.CensusStream;
using Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Utils;
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
        services.AddSingleton<IWebsocketHealthMonitor, WebsocketHealthMonitor>();
        services.AddSingleton<IEventValidator, EventValidator>();
        services.AddSingleton<IEventProcessorHandler, EventProcessorHandler>();

        foreach (var platform in configuration.GetCensusPlatforms())
        {
            services.AddWebsocketMonitor(platform);
        }

        services.AddStatefulHostedService<ICharacterUpdaterService, CharacterUpdaterService>();

        return services;
    }

    /// <summary>
    /// The PC monitor keeps the name it had before platforms were added, so its saved enabled state carries over.
    /// </summary>
    public static string GetWebsocketMonitorName(Platform platform)
    {
        return platform == Platform.PC ? nameof(WebsocketMonitor) : $"{nameof(WebsocketMonitor)}-{platform}";
    }

    private static void AddWebsocketMonitor(this IServiceCollection services, Platform platform)
    {
        services.AddStatefulHostedService(GetWebsocketMonitorName(platform), (sp, state) =>
        {
            return new WebsocketMonitor(
                platform,
                sp.GetRequiredKeyedService<ICensusStreamClient>(platform.GetCensusServiceKey()),
                sp.GetRequiredService<IWebsocketEventHandler>(),
                sp.GetRequiredService<IWorldMonitor>(),
                sp.GetRequiredService<IWebsocketHealthMonitor>(),
                sp.GetRequiredService<IOptions<LiveOptions>>(),
                state,
                sp.GetRequiredService<ILogger<WebsocketMonitor>>());
        });
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
