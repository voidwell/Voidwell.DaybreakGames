using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Voidwell.DaybreakGames.Utils.HostedService;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStatefulServiceDependencies(this IServiceCollection services)
    {
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
        where TService : class
        where TImplementation : class, TService, IStatefulHostedService
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

    /// <summary>
    /// Registers one named instance of a service type that can run several times side by side.
    /// Each instance is resolved as a keyed service under <paramref name="serviceName"/> and gets its own state.
    /// </summary>
    public static IServiceCollection AddStatefulHostedService<TImplementation>(this IServiceCollection services, string serviceName,
        Func<IServiceProvider, HostedServiceState, TImplementation> implementationFactory)
        where TImplementation : class, IStatefulHostedService
    {
        services.AddKeyedSingleton<HostedServiceState>(serviceName, (sp, _) =>
        {
            return new NamedHostedServiceState(serviceName, () => sp.GetRequiredKeyedService<TImplementation>(serviceName));
        });

        services.AddKeyedSingleton(serviceName, (sp, _) =>
        {
            return implementationFactory(sp, sp.GetRequiredKeyedService<HostedServiceState>(serviceName));
        });

        services.AddSingleton(sp =>
        {
            return sp.GetRequiredKeyedService<HostedServiceState>(serviceName);
        });

        // Not AddHostedService: it de-duplicates factory registrations by type, which would drop every instance but the first.
        services.AddSingleton<IHostedService>(sp =>
        {
            return new StatefulHostedServiceWrapper<TImplementation>(sp.GetRequiredKeyedService<TImplementation>(serviceName));
        });

        return services;
    }
}
