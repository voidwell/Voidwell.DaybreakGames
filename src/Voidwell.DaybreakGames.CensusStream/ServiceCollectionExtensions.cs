using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;
using Voidwell.DaybreakGames.CensusStream.EventProcessors;

namespace Voidwell.DaybreakGames.CensusStream
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCensusStreamServices(this IServiceCollection services)
        {
            services.AddEventProcessors();

            services.AddTransient<IStreamClient, StreamClient>();
            services.AddSingleton<IWebsocketEventHandler, WebsocketEventHandler>();
            services.AddSingleton<IWebsocketMonitor, WebsocketMonitor>();
            services.AddSingleton<IWebsocketHealthMonitor, WebsocketHealthMonitor>();
            services.AddSingleton<IEventValidator, EventValidator>();
            services.AddSingleton<IEventProcessorHandler, EventProcessorHandler>();

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
