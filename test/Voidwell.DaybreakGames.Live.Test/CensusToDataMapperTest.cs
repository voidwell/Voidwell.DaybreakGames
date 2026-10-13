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

        Assert.Equal(1, result.PopulationVs);
        Assert.Equal(2, result.PopulationNc);
        Assert.Equal(3, result.PopulationTr);
    }

    [Fact]
    public void GainExperience_AssignsAFreshIdPerMapping()
    {
        var payload = new GainExperience { CharacterId = "c1", ExperienceId = 5 };

        var first = CensusToDataMapper.Map(payload);
        var second = CensusToDataMapper.Map(payload);

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(second.Id, first.Id);
        Assert.Equal("c1", first.CharacterId);
    }

    [Fact]
    public void MetagameEvent_MapsZoneControlFromFactionValues()
    {
        var result = CensusToDataMapper.Map(new MetagameEvent { FactionVs = 10, FactionNc = 20, FactionTr = 30 });

        Assert.Equal(10, result.ZoneControlVs);
        Assert.Equal(20, result.ZoneControlNc);
        Assert.Equal(30, result.ZoneControlTr);
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

        Assert.Equal(99, alert.MetagameInstanceId);
        Assert.Equal(timestamp, alert.StartDate);
        Assert.Equal(timestamp.AddMinutes(45), alert.EndDate);
        Assert.Equal(1, alert.StartFactionVs);
        Assert.Equal(2, alert.LastFactionNc);
        Assert.Equal(3, alert.LastFactionTr);
        Assert.Null(alert.MetagameEvent);
    }

    [Theory]
    [InlineData("0", null)]
    [InlineData("123", "123")]
    [InlineData(null, null)]
    public void PlayerFacilityEvents_TreatZeroOutfitIdAsNone(string outfitId, string expected)
    {
        Assert.Equal(expected, CensusToDataMapper.Map(new PlayerFacilityCapture { OutfitId = outfitId }).OutfitId);
        Assert.Equal(expected, CensusToDataMapper.Map(new PlayerFacilityDefend { OutfitId = outfitId }).OutfitId);
    }
}
