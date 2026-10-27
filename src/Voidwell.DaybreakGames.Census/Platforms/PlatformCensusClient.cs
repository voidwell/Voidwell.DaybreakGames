using DaybreakGames.Census;
using DaybreakGames.Census.Operators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Voidwell.DaybreakGames.Census.Platforms;

/// <summary>
/// Sends each call to the Census client of the platform set on <see cref="ICensusContext"/>,
/// so services that hold an <see cref="ICensusClient"/> follow the platform of the flow calling them.
/// </summary>
public class PlatformCensusClient : ICensusClient
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICensusContext _censusContext;
    private readonly CensusPlatformOptions _options;

    public PlatformCensusClient(IServiceProvider serviceProvider, ICensusContext censusContext, IOptions<CensusPlatformOptions> options)
    {
        _serviceProvider = serviceProvider;
        _censusContext = censusContext;
        _options = options.Value;
    }

    private ICensusClient Current
    {
        get
        {
            var platform = _censusContext.Platform ?? _options.DefaultPlatform;
            return _serviceProvider.GetRequiredKeyedService<ICensusClient>(platform.GetCensusServiceKey());
        }
    }

    public CensusQuery CreateQuery(string serviceName)
    {
        return Current.CreateQuery(serviceName);
    }

    public Task<T> ExecuteQueryAsync<T>(CensusQuery query, CancellationToken cancellationToken = default)
    {
        return Current.ExecuteQueryAsync<T>(query, cancellationToken);
    }

    public Task<IEnumerable<T>> ExecuteQueryListAsync<T>(CensusQuery query, CancellationToken cancellationToken = default)
    {
        return Current.ExecuteQueryListAsync<T>(query, cancellationToken);
    }

    public Task<IEnumerable<T>> ExecuteQueryBatchAsync<T>(CensusQuery query, CancellationToken cancellationToken = default)
    {
        return Current.ExecuteQueryBatchAsync<T>(query, cancellationToken);
    }

    public Uri CreateRequestUri(CensusQuery query)
    {
        return Current.CreateRequestUri(query);
    }
}
