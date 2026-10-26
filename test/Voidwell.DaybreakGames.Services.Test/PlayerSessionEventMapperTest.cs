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
            CharacterId = 1001,
            Character = new Character { Name = "Victim", FactionId = 2 },
            AttackerCharacterId = 1002,
            AttackerCharacter = new Character { Name = "Attacker", FactionId = 1 },
            AttackerWeaponId = 80,
            AttackerWeapon = new Item { Name = "Gauss Rifle", ImageId = 7 },
            IsHeadshot = true,
            AttackerFireModeId = 3,
            AttackerLoadoutId = 4,
            AttackerOutfitId = 101,
            AttackerVehicleId = 5,
            CharacterLoadoutId = 6,
            CharacterOutfitId = 102
        };

        var result = Assert.IsType<PlayerSessionDeathEvent>(PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { death }).Single());

        Assert.Equal(_time, result.Timestamp);
        Assert.Equal(2, result.ZoneId);
        Assert.Equal("1002", result.Attacker.Id);
        Assert.Equal("Attacker", result.Attacker.Name);
        Assert.Equal(1, result.Attacker.FactionId);
        Assert.Equal("1001", result.Victim.Id);
        Assert.Equal("Victim", result.Victim.Name);
        Assert.Equal(2, result.Victim.FactionId);
        Assert.Equal(80, result.Weapon.Id);
        Assert.Equal("Gauss Rifle", result.Weapon.Name);
        Assert.Equal(7, result.Weapon.ImageId);
        Assert.True(result.IsHeadshot);
        Assert.Equal(3, result.AttackerFireModeId);
        Assert.Equal(4, result.AttackerLoadoutId);
        Assert.Equal(101, result.AttackerOutfitId);
        Assert.Equal(5, result.AttackerVehicleId);
        Assert.Equal(6, result.CharacterLoadoutId);
        Assert.Equal(102, result.CharacterOutfitId);
    }

    [Fact]
    public void Death_MissingNavigationData_ProducesNullNames()
    {
        var death = new Death { CharacterId = 1001, AttackerCharacterId = 1002, AttackerWeaponId = 80 };

        var result = (PlayerSessionDeathEvent)PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { death }).Single();

        Assert.Null(result.Attacker.Name);
        Assert.Null(result.Attacker.FactionId);
        Assert.Null(result.Victim.Name);
        Assert.Null(result.Weapon.Name);
        Assert.Null(result.Weapon.ImageId);
    }

    [Fact]
    public void Death_WithoutAttackerWeapon_Throws()
    {
        var death = new Death { CharacterId = 1001, AttackerWeaponId = null };

        var act = () => PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { death }).ToList();

        Assert.ThrowsAny<InvalidOperationException>(act);
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

        var result = Assert.IsType<PlayerSessionFacilityCaptureEvent>(PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { capture }).Single());

        Assert.Equal(_time, result.Timestamp);
        Assert.Equal(4, result.ZoneId);
        Assert.Equal(9, result.Facility.Id);
        Assert.Equal("Peris", result.Facility.Name);
        Assert.Equal(5, result.Facility.TypeId);
        Assert.Equal("Large Outpost", result.Facility.TypeName);
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

        var result = Assert.IsType<PlayerSessionFacilityDefendEvent>(PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { defend }).Single());

        Assert.Equal(9, result.Facility.Id);
        Assert.Equal("Peris", result.Facility.Name);
    }

    [Fact]
    public void BattleRankUp_MapsRank()
    {
        var result = Assert.IsType<PlayerSessionBattleRankUpEvent>(PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { new BattlerankUp { Timestamp = _time, ZoneId = 2, BattleRank = 42 } }).Single());

        Assert.Equal(42, result.BattleRank);
        Assert.Equal(_time, result.Timestamp);
    }

    [Fact]
    public void VehicleDestroy_MapsVehicleAndFacility()
    {
        var destroy = new VehicleDestroy
        {
            Timestamp = _time,
            ZoneId = 2,
            CharacterId = 1001,
            AttackerCharacterId = 1002,
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

        var result = Assert.IsType<PlayerSessionVehicleDestroyEvent>(PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { destroy }).Single());

        Assert.Equal(3, result.VictimVehicle.Id);
        Assert.Equal("Harasser", result.VictimVehicle.Name);
        Assert.Equal(11, result.VictimVehicle.ImageId);
        Assert.Equal("Lancer", result.Weapon.Name);
        Assert.Equal(2, result.FactionId);
        Assert.Equal(5, result.AttackerVehicleId);
        Assert.Equal(9, result.Facility.Id);
        Assert.Equal("Peris", result.Facility.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    public void VehicleDestroy_NoFacility_LeavesFacilityEmpty(int? facilityId)
    {
        var destroy = new VehicleDestroy { AttackerWeaponId = 1, VehicleId = 1, FacilityId = facilityId };

        var result = (PlayerSessionVehicleDestroyEvent)PlayerSessionEventMapper.ToPlayerSessionEvent(new[] { destroy }).Single();

        Assert.Null(result.Facility);
    }

    [Fact]
    public void Mapping_EmptyInput_ReturnsEmpty()
    {
        Assert.Empty(PlayerSessionEventMapper.ToPlayerSessionEvent(Array.Empty<Death>()));
        Assert.Empty(PlayerSessionEventMapper.ToPlayerSessionEvent(Array.Empty<VehicleDestroy>()));
    }
}
