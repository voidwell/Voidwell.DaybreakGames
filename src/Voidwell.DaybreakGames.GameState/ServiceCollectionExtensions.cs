using Microsoft.Extensions.DependencyInjection;

namespace Voidwell.DaybreakGames.GameState
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameStateServices(this IServiceCollection services)
        {
            services.AddSingleton<IPlayerMonitor, PlayerMonitor>();
            services.AddSingleton<IZoneMonitor, ZoneMonitor>();
            services.AddSingleton<IWorldMonitor, WorldMonitor>();

            return services;
        }
    }
}