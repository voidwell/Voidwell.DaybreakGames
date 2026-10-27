using DaybreakGames.Census;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Census.Patcher;

/// <summary>
/// Sanctuary only mirrors PC data, which is also what the static collections it patches use on every platform.
/// </summary>
public class SanctuaryCensusClient : CensusClient, ICensusPatchClient
{
    private const string _sanctuaryEndpoint = "census.lithafalcon.cc";
    private const Platform _sanctuaryPlatform = Platform.PC;

    public SanctuaryCensusClient(IOptionsMonitor<CensusOptions> options, ILogger<SanctuaryCensusClient> logger, IHttpClientFactory httpClientFactory)
        : base(
            httpClientFactory.CreateClient(Microsoft.Extensions.DependencyInjection.CensusServiceExtensions.GetCensusHttpClientName(_sanctuaryPlatform.GetCensusServiceKey())),
            Options.Create(
                new CensusOptions
                {
                    CensusApiEndpoint = _sanctuaryEndpoint,
                    CensusServiceNamespace = _sanctuaryPlatform.GetCensusNamespace(),
                    CensusServiceId = options.Get(_sanctuaryPlatform.GetCensusServiceKey()).CensusServiceId
                }),
            logger)
    {
    }
}
