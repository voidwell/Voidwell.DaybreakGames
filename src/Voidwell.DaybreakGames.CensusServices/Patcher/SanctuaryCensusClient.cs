using DaybreakGames.Census;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Voidwell.DaybreakGames.Census.Patcher
{
    public class SanctuaryCensusClient : CensusClient, ICensusPatchClient
    {
        private const string SanctuaryEndpoint = "census.lithafalcon.cc";
        private const string SanctuaryNamespace = "ps2";

        public SanctuaryCensusClient(IOptions<CensusOptions> options, ILogger<SanctuaryCensusClient> logger)
            : base(Options.Create(
                new CensusOptions
                {
                    CensusApiEndpoint = SanctuaryEndpoint,
                    CensusServiceNamespace = SanctuaryNamespace,
                    CensusServiceId = options.Value.CensusServiceId
                }), logger)
        {
        }
    }
}
