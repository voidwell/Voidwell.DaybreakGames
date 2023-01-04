using DaybreakGames.Census;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Voidwell.DaybreakGames.CensusServices.Patcher;
using Xunit;

namespace Voidwell.DaybreakGames.Test.CensusServicesTests
{
    public class PatchClientTest
    {
        [Fact]
        public void SanctuaryCensusClient_expectedUri()
        {
            var options = Options.Create<CensusOptions>(new CensusOptions { CensusServiceId = "testtest"});
            var logger = new LoggerFactory().CreateLogger<SanctuaryCensusClient>();

            var testClient = new SanctuaryCensusClient(options, logger);

            var query = testClient.CreateQuery("zone");

            var resultUri = query.GetUri().ToString();

            Assert.Equal("http://census.lithafalcon.cc/s:testtest/get/ps2/zone/", resultUri);
        }
    }
}
