using FluentAssertions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class PlayerSessionEventMapperTest
{
    private static readonly DateTime _time = new(2024, 6, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Death_MapsParticipantsWeaponAndFlags()
    {
        var death = new Death
        {
            Timestamp = _time,
            ZoneId = 2,
            CharacterId = "victim",
            Character = new Character { Name = "Victim", FactionId = 2 },
            AttackerCharacterId = "attacker",
            AttackerCharacter = new Character { Name = "Attacker", FactionId = 1 },
            AttackerWeaponId = 80,
            AttackerWeapon = new Item { Name = "Gauss Rifle", ImageId = 7 },
            IsHeadshot = true,
            AttackerFireModeId = 3,
            AttackerLoadoutId = 4,
            AttackerOutfitId = "o1",
            AttackerVehicleId = 5,
            CharacterLoadoutId = 6,
            CharacterOutfitId = "o2"
        };

        var result = PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { death }).Single().Should().BeOfType<PlayerSessionDeathEvent>().Subject;

        result.Timestamp.Should().Be(_time);
        result.ZoneId.Should().Be(2);
        result.Attacker.Id.Should().Be("attacker");
        result.Attacker.Name.Should().Be("Attacker");
        result.Attacker.FactionId.Should().Be(1);
        result.Victim.Id.Should().Be("victim");
        result.Victim.Name.Should().Be("Victim");
        result.Victim.FactionId.Should().Be(2);
        result.Weapon.Id.Should().Be(80);
        result.Weapon.Name.Should().Be("Gauss Rifle");
        result.Weapon.ImageId.Should().Be(7);
        result.IsHeadshot.Should().BeTrue();
        result.AttackerFireModeId.Should().Be(3);
        result.AttackerLoadoutId.Should().Be(4);
        result.AttackerOutfitId.Should().Be("o1");
        result.AttackerVehicleId.Should().Be(5);
        result.CharacterLoadoutId.Should().Be(6);
        result.CharacterOutfitId.Should().Be("o2");
    }

    [Fact]
    public void Death_MissingNavigationData_ProducesNullNames()
    {
        var death = new Death { CharacterId = "victim", AttackerCharacterId = "attacker", AttackerWeaponId = 80 };

        var result = (PlayerSessionDeathEvent)PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { death }).Single();

        result.Attacker.Name.Should().BeNull();
        result.Attacker.FactionId.Should().BeNull();
        result.Victim.Name.Should().BeNull();
        result.Weapon.Name.Should().BeNull();
        result.Weapon.ImageId.Should().BeNull();
    }

    [Fact]
    public void Death_WithoutAttackerWeapon_Throws()
    {
        var death = new Death { CharacterId = "victim", AttackerWeaponId = null };

        var act = () => PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { death }).ToList();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void FacilityCapture_MapsFacility()
    {
        var capture = new PlayerFacilityCapture
        {
            Timestamp = _time,
            ZoneId = 4,
            FacilityId = 9,
            Facility = new MapRegion { FacilityName = "Peris", FacilityTypeId = 5, FacilityType = "Large Outpost" }
        };

        var result = PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { capture }).Single().Should().BeOfType<PlayerSessionFacilityCaptureEvent>().Subject;

        result.Timestamp.Should().Be(_time);
        result.ZoneId.Should().Be(4);
        result.Facility.Id.Should().Be(9);
        result.Facility.Name.Should().Be("Peris");
        result.Facility.TypeId.Should().Be(5);
        result.Facility.TypeName.Should().Be("Large Outpost");
    }

    [Fact]
    public void FacilityDefend_MapsFacility()
    {
        var defend = new PlayerFacilityDefend
        {
            Timestamp = _time,
            ZoneId = 4,
            FacilityId = 9,
            Facility = new MapRegion { FacilityName = "Peris", FacilityTypeId = 5, FacilityType = "Large Outpost" }
        };

        var result = PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { defend }).Single().Should().BeOfType<PlayerSessionFacilityDefendEvent>().Subject;

        result.Facility.Id.Should().Be(9);
        result.Facility.Name.Should().Be("Peris");
    }

    [Fact]
    public void BattleRankUp_MapsRank()
    {
        var result = PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { new BattlerankUp { Timestamp = _time, ZoneId = 2, BattleRank = 42 } })
            .Single().Should().BeOfType<PlayerSessionBattleRankUpEvent>().Subject;

        result.BattleRank.Should().Be(42);
        result.Timestamp.Should().Be(_time);
    }

    [Fact]
    public void VehicleDestroy_MapsVehicleAndFacility()
    {
        var destroy = new VehicleDestroy
        {
            Timestamp = _time,
            ZoneId = 2,
            CharacterId = "victim",
            AttackerCharacterId = "attacker",
            AttackerWeaponId = 80,
            AttackerWeapon = new Item { Name = "Lancer" },
            VehicleId = 3,
            VictimVehicle = new Vehicle { Name = "Harasser", ImageId = 11 },
            AttackerLoadoutId = 4,
            AttackerVehicleId = 5,
            FactionId = 2,
            FacilityId = 9,
            Facility = new MapRegion { FacilityName = "Peris" }
        };

        var result = PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { destroy }).Single().Should().BeOfType<PlayerSessionVehicleDestroyEvent>().Subject;

        result.VictimVehicle.Id.Should().Be(3);
        result.VictimVehicle.Name.Should().Be("Harasser");
        result.VictimVehicle.ImageId.Should().Be(11);
        result.Weapon.Name.Should().Be("Lancer");
        result.FactionId.Should().Be(2);
        result.AttackerVehicleId.Should().Be(5);
        result.Facility.Id.Should().Be(9);
        result.Facility.Name.Should().Be("Peris");
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void VehicleDestroy_NoFacility_LeavesFacilityEmpty(int? facilityId)
    {
        var destroy = new VehicleDestroy { AttackerWeaponId = 1, VehicleId = 1, FacilityId = facilityId };

        var result = (PlayerSessionVehicleDestroyEvent)PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { destroy }).Single();

        result.Facility.Should().BeNull();
    }

    [Fact]
    public void Mapping_EmptyInput_ReturnsEmpty()
    {
        PlayerSessionEventMapper.ToPlayerSessionEvent(Array.Empty<Death>()).Should().BeEmpty();
        PlayerSessionEventMapper.ToPlayerSessionEvent(Array.Empty<VehicleDestroy>()).Should().BeEmpty();
    }
}
