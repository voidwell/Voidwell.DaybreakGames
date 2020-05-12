using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Voidwell.DaybreakGames.Utils.StatefulHostedService
{
    public static class ServiceExtensions
    {
        public static void AddStatefulHostedService<TStatefulHostedService>(this IServiceCollection services)
            where TStatefulHostedService : class, IStatefulHostedService
        {
            services.AddHostedService<TStatefulHostedService>();
        }

        public static IEnumerable<IStatefulHostedService> GetStatefulHostedServices(this IServiceProvider serviceProvider)
        {
            var services = serviceProvider.GetRequiredService<IServiceCollection>();

            var statefulHostedServices = services.Where(a =>
                typeof(IHostedService).IsAssignableFrom(a.ServiceType)
                && typeof(IStatefulHostedService).IsAssignableFrom(a.ImplementationType)
                && a.ImplementationType.IsClass
                && !a.ImplementationType.IsAbstract);

            return statefulHostedServices.Select(a => a.ImplementationFactory(serviceProvider) as IStatefulHostedService);
        }
    }
}
