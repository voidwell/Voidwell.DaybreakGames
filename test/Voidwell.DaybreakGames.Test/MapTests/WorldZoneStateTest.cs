using Xunit;

namespace Voidwell.DaybreakGames.Test.MapTests
{
    public class WorldZoneStateTest
    {
        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(8)]
        public void WorldZoneState_CalculateScore(int zoneId)
        {
            var worldZoneState = MapHelper.GetWorldZoneState(zoneId);

            var score = worldZoneState.MapScore;

            var percentTotal = score.Territories.Neutural.Percent + score.Territories.Vs.Percent + score.Territories.Nc.Percent + score.Territories.Tr.Percent;
            var connectedPercentTotal = score.ConnectedTerritories.Neutural.Percent + score.ConnectedTerritories.Vs.Percent + score.ConnectedTerritories.Nc.Percent + score.ConnectedTerritories.Tr.Percent;

            Assert.Equal(1, percentTotal);
            Assert.Equal(1, connectedPercentTotal);
        }
    }
}
