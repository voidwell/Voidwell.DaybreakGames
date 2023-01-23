using System;
using System.Threading.Tasks;
using Moq;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Test
{
    public class MetagameEventProcessorTest : IClassFixture<MetagameEventProcessorFixture>
    {
        private readonly MetagameEventProcessorFixture _fixture;

        public MetagameEventProcessorTest(MetagameEventProcessorFixture fixture)
        {
            _fixture = fixture;
            _fixture.ResetFixture();
        }

        [Fact]
        public async Task StartAlert_SetsAlertState()
        {
            var timestamp = DateTime.UtcNow;
            var metagameEventId = 123;
            var metagameInstanceId = 456;
            var worldId = 2;
            var zoneId = 4;
            var zoneMetagameEvent = new ZoneMetagameEvent
            {
                Duration = TimeSpan.FromMinutes(13),
                Id = metagameEventId,
                Name = "My metagame event",
                TypeId = 1,
                ZoneId = zoneId
            };
            var metagameEvent = new MetagameEvent
            {
                MetagameEventState = "STARTED",
                MetagameEventId = metagameEventId,
                InstanceId = metagameInstanceId,
                WorldId = worldId,
                Timestamp = timestamp
            };

            _fixture.MetagameEventService.AsMock()
                .Setup(a => a.GetMetagameEvent(metagameEventId))
                .ReturnsAsync(zoneMetagameEvent);

            _fixture.WorldMonitor.AsMock()
                .Setup(a => a.UpdateZoneAlert(worldId, zoneId, It.Is<ZoneAlertState>(b => b.Timestamp == timestamp && b.InstanceId == 45618 && b.MetagameEvent == zoneMetagameEvent)))
                .Verifiable("failed to update world state");

            var sut = _fixture.CreateSut();

            await sut.Process(metagameEvent);

            _fixture.WorldMonitor.AsMock()
                .Verify();
        }
    }
}
