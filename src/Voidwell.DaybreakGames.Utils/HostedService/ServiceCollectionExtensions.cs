using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Voidwell.DaybreakGames.Utils.HostedService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStatefulServiceDependencies(this IServiceCollection services)
        {
            services.TryAddSingleton<IStatefulHostedServiceManager, StatefulHostedServiceManager>();

            services.AddHostedService(sp =>
            {
                return sp.GetRequiredService<IStatefulHostedServiceManager>();
            });
            
            return services;
        }

        public static IServiceCollection AddStatefulHostedService<TImplementation>(this IServiceCollection services)
            where TImplementation : class, IStatefulHostedService
        {
            services.AddStatefulServiceDependencies();

            services.AddSingleton<TImplementation>();

            services.AddSingleton(sp =>
            {
                return new HostedServiceState<TImplementation> { Service = sp.GetRequiredService<TImplementation>() };
            });
            services.AddSingleton(sp =>
            {
                return (HostedServiceState)sp.GetRequiredService<HostedServiceState<TImplementation>>();
            });

            services.AddHostedService<StatefulHostedServiceWrapper<TImplementation>>();

            return services;
        }

        public static IServiceCollection AddStatefulHostedService<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService, IStatefulHostedService
        {
            services.AddStatefulServiceDependencies();

            services.AddSingleton<TService, TImplementation>();

            services.AddSingleton(sp =>
            {
                return new HostedServiceState<TImplementation> { Service = (TImplementation)sp.GetRequiredService<TService>() };
            });
            services.AddSingleton(sp =>
            {
                return (HostedServiceState)sp.GetRequiredService<HostedServiceState<TImplementation>>();
            });

            services.AddHostedService(sp =>
            {
                return new StatefulHostedServiceWrapper<TImplementation>((TImplementation)sp.GetRequiredService<TService>());
            });

            return services;
        }
    }
}
