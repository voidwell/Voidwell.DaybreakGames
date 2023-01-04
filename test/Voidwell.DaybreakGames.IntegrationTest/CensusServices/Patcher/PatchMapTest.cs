using DaybreakGames.Census;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Patcher;
using Voidwell.DaybreakGames.CensusServices.Patcher.Services;
using Xunit;

namespace Voidwell.DaybreakGames.IntegrationTest.CensusServices.Patcher
{
    public class PatchMapTest
    {
        private readonly IPatchClient _patchClient;
        private readonly CensusMap _serviceClient;

        public PatchMapTest()
        {
            var options = Options.Create<CensusOptions>(new CensusOptions { CensusServiceId = "example" });

            var patchLogger = new LoggerFactory().CreateLogger<SanctuaryCensusClient>();
            _patchClient = new SanctuaryCensusClient(options, patchLogger);

            var censusLogger = new LoggerFactory().CreateLogger<CensusClient>();
            var censusClient = new CensusClient(options, censusLogger);
            _serviceClient = new CensusMap(censusClient);
        }

        [Fact]
        public async Task GetAllMapRegions()
        {
            var sut = new PatchMap(_patchClient, _serviceClient);

            var result = await sut.GetAllMapRegions();

            Assert.NotEmpty(result);
        }
    }
}
