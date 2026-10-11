using FluentAssertions;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.Mappers;
using Xunit;

namespace Voidwell.DaybreakGames.Live.Test;

public class CensusToDataMapperTest
{
    [Fact]
    public void ContinentLock_MapsPopulationsOntoRenamedMembers()
    {
        var result = CensusToDataMapper.Map(new ContinentLock { VsPopulation = 1, NcPopulation = 2, TrPopulation = 3 });

        result.PopulationVs.Should().Be(1);
        result.PopulationNc.Should().Be(2);
        result.PopulationTr.Should().Be(3);
    }

    [Fact]
    public void GainExperience_AssignsAFreshIdPerMapping()
    {
        var payload = new GainExperience { CharacterId = "c1", ExperienceId = 5 };

        var first = CensusToDataMapper.Map(payload);
        var second = CensusToDataMapper.Map(payload);

        first.Id.Should().NotBeEmpty();
        first.Id.Should().NotBe(second.Id);
        first.CharacterId.Should().Be("c1");
    }

    [Fact]
    public void MetagameEvent_MapsZoneControlFromFactionValues()
    {
        var result = CensusToDataMapper.Map(new MetagameEvent { FactionVs = 10, FactionNc = 20, FactionTr = 30 });

        result.ZoneControlVs.Should().Be(10);
        result.ZoneControlNc.Should().Be(20);
        result.ZoneControlTr.Should().Be(30);
    }

    [Fact]
    public void MetagameEvent_MapsToAlertWithFortyFiveMinuteDuration()
    {
        var timestamp = new DateTime(2024, 5, 1, 12, 0, 0, DateTimeKind.Utc);

        var alert = CensusToDataMapper.MapToAlert(new MetagameEvent
        {
            InstanceId = 99,
            Timestamp = timestamp,
            FactionVs = 1,
            FactionNc = 2,
            FactionTr = 3
        });

        alert.MetagameInstanceId.Should().Be(99);
        alert.StartDate.Should().Be(timestamp);
        alert.EndDate.Should().Be(timestamp.AddMinutes(45));
        alert.StartFactionVs.Should().Be(1);
        alert.LastFactionNc.Should().Be(2);
        alert.LastFactionTr.Should().Be(3);
        alert.MetagameEvent.Should().BeNull();
    }

    [Theory]
    [InlineData("0", null)]
    [InlineData("123", "123")]
    [InlineData(null, null)]
    public void PlayerFacilityEvents_TreatZeroOutfitIdAsNone(string outfitId, string expected)
    {
        CensusToDataMapper.Map(new PlayerFacilityCapture { OutfitId = outfitId }).OutfitId.Should().Be(expected);
        CensusToDataMapper.Map(new PlayerFacilityDefend { OutfitId = outfitId }).OutfitId.Should().Be(expected);
    }
}
