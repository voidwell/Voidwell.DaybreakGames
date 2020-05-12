using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.GameState.CensusStream;
using Voidwell.DaybreakGames.GameState.Services;

namespace Voidwell.DaybreakGames.GameState
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureGameStateServices(this IServiceCollection services)
        {
            services.AddSingleton<IWebsocketEventHandler, WebsocketEventHandler>();
            services.AddSingleton<IWebsocketMonitor, WebsocketMonitor>();

            services.AddSingleton<IWorldMonitor, WorldMonitor>();
            services.AddSingleton<IPlayerMonitor, PlayerMonitor>();
            services.AddSingleton<IMetagameEventMonitor, MetagameEventMonitor>();
        }
    }
}
