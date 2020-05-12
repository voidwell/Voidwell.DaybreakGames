using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Voidwell.DaybreakGames.UpdatableServices;
using Voidwell.DaybreakGames.Utils.StatefulHostedService;
using Voidwell.DaybreakGames.App.HostedServices;

namespace Voidwell.DaybreakGames.App
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<AppOptions>(configuration);

            services.AddUpdateableTasks();

            services.AddHostedService<WebsocketMonitorHostedService>();
            services.AddHostedService<StoreUpdaterSchedulerHostedService>();
            services.AddStatefulHostedService<WebsocketMonitorHostedService>();
            services.AddStatefulHostedService<CharacterUpdaterProcessor>();
        }

        private static void AddUpdateableTasks(this IServiceCollection services)
        {
            var updatableTypes = typeof(IUpdatable).GetTypeInfo().Assembly.GetTypes()
                    .Where(a => typeof(IUpdatable).IsAssignableFrom(a) && a.GetTypeInfo().IsClass && !a.GetTypeInfo().IsAbstract)
                    .ToList();

            updatableTypes.ForEach(t => services.AddTransient(typeof(IUpdatable), t));
        }
    }
}
