﻿using AsyncKeyedLock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Voidwell.DaybreakGames.Utils.HostedService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStatefulServiceDependencies(this IServiceCollection services)
        {
            services.TryAddSingleton<IStatefulHostedServiceManager, StatefulHostedServiceManager>();
            services.AddSingleton(new AsyncKeyedLocker<string>(o =>
            {
                o.PoolSize = 20;
                o.PoolInitialFill = 1;
            }));

            services.AddSingleton<IStatefulHostedServiceManager, StatefulHostedServiceManager>();

            services.AddHostedService(sp =>
            {
                return (StatefulHostedServiceManager)sp.GetRequiredService<IStatefulHostedServiceManager>();
            });

            services.AddSingleton(typeof(HostedServiceState<>));

            return services;
        }

        public static IServiceCollection AddStatefulHostedService<TImplementation>(this IServiceCollection services)
            where TImplementation : class, IStatefulHostedService
        {
            services.AddSingleton<TImplementation>();

            services.AddSingleton(sp =>
            {
                return (HostedServiceState)sp.GetRequiredService<HostedServiceState<TImplementation>>();
            });

            services.AddHostedService<StatefulHostedServiceWrapper<TImplementation>>();

            return services;
        }

        public static IServiceCollection AddStatefulHostedService<TService, TImplementation>(this IServiceCollection services)
            where TService : class, IStatefulHostedService
            where TImplementation : class, TService
        {
            services.AddSingleton<TService, TImplementation>();

            services.AddSingleton(sp =>
            {
                return (HostedServiceState)sp.GetRequiredService<HostedServiceState<TService>>();
            });

            services.AddHostedService(sp =>
            {
                return new StatefulHostedServiceWrapper<TImplementation>((TImplementation)sp.GetRequiredService<TService>());
            });

            return services;
        }
    }
}
