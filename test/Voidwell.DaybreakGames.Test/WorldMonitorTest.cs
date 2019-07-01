using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Test.MapTests;
using Xunit;

namespace Voidwell.DaybreakGames.Test
{
    public class WorldMonitorTest : IClassFixture<WorldMonitorFixture>
    {
        private readonly WorldMonitorFixture _fixture;

        public WorldMonitorTest(WorldMonitorFixture fixture)
        {
            _fixture = fixture;
            _fixture.ResetFixture();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SetWorldState_HasWorldState(bool isOnline)
        {
            int worldId = 2;
            int zoneId = 2;
            var testZone = new Zone
            {
                Id = zoneId,
                Name = "Test Zone"
            };
            var mapOwnership = MapHelper.GetMapOwnership(zoneId);
            var zoneMap = MapHelper.GetZoneMap(zoneId);

            _fixture.ZoneService.AsMock()
                .Setup(a => a.GetPlayableZones())
                .ReturnsAsync(new[] { testZone });

            _fixture.MapService.AsMock()
                .Setup(a => a.GetMapOwnership(worldId, zoneId))
                .ReturnsAsync(mapOwnership);

            _fixture.MapService.AsMock()
                .Setup(a => a.GetZoneMap(zoneId))
                .ReturnsAsync(zoneMap);

            _fixture.MapService.AsMock()
                .Setup(a => a.GetZoneStateHistoricals())
                .ReturnsAsync(new ZoneStateHistorical(Enumerable.Empty<ContinentLock>(), Enumerable.Empty<ContinentUnlock>()));

            var sut = _fixture.CreateSut();

            await sut.SetWorldState(worldId, "test", isOnline);

            var worldState = await sut.GetWorldState(worldId);

            worldState.Should()
                .NotBeNull();
            worldState.Id.Should()
                .Be(worldId);
            worldState.IsOnline.Should()
                .Be(isOnline);
        }

        [Fact]
        public async Task AddAlertToWorldState_AlertInState()
        {
            int worldId = 2;
            int zoneId = 2;
            var testZone = new Zone
            {
                Id = zoneId,
                Name = "Test Zone"
            };
            var mapOwnership = MapHelper.GetMapOwnership(zoneId);
            var zoneMap = MapHelper.GetZoneMap(zoneId);

            var testZoneMetagameEvent = new ZoneMetagameEvent {Id = 1, Description = "my alert", Duration = TimeSpan.FromHours(3)};
            var testZoneAlertState = new ZoneAlertState(DateTime.UtcNow, 1234, testZoneMetagameEvent);
            
            _fixture.ZoneService.AsMock()
                .Setup(a => a.GetPlayableZones())
                .ReturnsAsync(new[] { testZone });

            _fixture.MapService.AsMock()
                .Setup(a => a.GetMapOwnership(worldId, zoneId))
                .ReturnsAsync(mapOwnership);

            _fixture.MapService.AsMock()
                .Setup(a => a.GetZoneMap(zoneId))
                .ReturnsAsync(zoneMap);

            _fixture.MapService.AsMock()
                .Setup(a => a.GetZoneStateHistoricals())
                .ReturnsAsync(new ZoneStateHistorical(Enumerable.Empty<ContinentLock>(), Enumerable.Empty<ContinentUnlock>()));

            var sut = _fixture.CreateSut();

            await sut.SetWorldState(worldId, "test", true);

            sut.UpdateZoneAlert(worldId, zoneId, testZoneAlertState);

            var worldState = await sut.GetWorldState(worldId);

            worldState.ZoneStates.Should()
                .NotBeEmpty();
            worldState.ZoneStates.First().Id.Should()
                .Be(zoneId);
            worldState.ZoneStates.First().IsTracking.Should()
                .BeTrue();
            worldState.ZoneStates.First().AlertState.Should()
                .BeEquivalentTo(testZoneAlertState);
        }
    }
}
