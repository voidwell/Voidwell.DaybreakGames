using DaybreakGames.Census;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Voidwell.DaybreakGames.Census.Patcher;

public class SanctuaryCensusClient : CensusClient, ICensusPatchClient
{
    private const string _sanctuaryEndpoint = "census.lithafalcon.cc";
    private const string _sanctuaryNamespace = "ps2";

    public SanctuaryCensusClient(IOptions<CensusOptions> options, ILogger<SanctuaryCensusClient> logger)
        : base(Options.Create(
            new CensusOptions
            {
                CensusApiEndpoint = _sanctuaryEndpoint,
                CensusServiceNamespace = _sanctuaryNamespace,
                CensusServiceId = options.Value.CensusServiceId
            }), logger)
    {
    }
}
