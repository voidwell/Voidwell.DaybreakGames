using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Mappers;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class MappingProfilesTest
{
    private static readonly MapperConfiguration _configuration = new(cfg =>
    {
        cfg.AddProfile<CensusToDomainMappingProfile>();
        cfg.AddProfile<DataToDomainMappingProfile>();
        cfg.AddProfile<DomainToDomainMappingProfile>();
    }, NullLoggerFactory.Instance);

    private readonly IMapper _mapper = _configuration.CreateMapper();

    [Fact]
    public void Configuration_AllMapsCompile()
    {
        var act = () => _configuration.CompileMappings();

        act.Should().NotThrow();
    }

    [Fact]
    public void Character_MapsNavigationNamesOntoDetails()
    {
        var character = new Character
        {
            Id = "c1",
            Name = "Bob",
            Faction = new Faction { Name = "Vanu Sovereignty", ImageId = 12 },
            Title = new Title { Name = "Ace" },
            World = new World { Name = "Emerald" },
            WeaponStats = new List<CharacterWeaponStat>()
        };

        var details = _mapper.Map<CharacterDetails>(character);

        details.Faction.Should().Be("Vanu Sovereignty");
        details.FactionImageId.Should().Be(12);
        details.Title.Should().Be("Ace");
        details.World.Should().Be("Emerald");
    }

    [Fact]
    public void Character_VehicleStats_SeparatesGunnerAndPilotStatsPerVehicle()
    {
        var character = new Character
        {
            Id = "c1",
            WeaponStats = new List<CharacterWeaponStat>
            {
                new() { VehicleId = 5, ItemId = 100, Kills = 3, Deaths = 1 },
                new() { VehicleId = 5, ItemId = 101, Kills = 4, Deaths = 2 },
                new() { VehicleId = 5, ItemId = 0, Kills = 2, VehicleKills = 1 },
                new() { VehicleId = 0, ItemId = 200, Kills = 50 }
            }
        };

        var result = _mapper.Map<CharacterDetails>(character).VehicleStats.Should().ContainSingle().Subject;

        result.VehicleId.Should().Be(5);
        result.Kills.Should().Be(7);
        result.Deaths.Should().Be(3);
        result.PilotKills.Should().Be(2);
        result.PilotVehicleKills.Should().Be(1);
    }

    [Fact]
    public void LifetimeStats_MapsWeaponPrefixedColumnsToPlainNames()
    {
        var stats = new CharacterLifetimeStat
        {
            WeaponKills = 10,
            WeaponDeaths = 4,
            WeaponHeadshots = 3,
            WeaponPlayTime = 600,
            WeaponVehicleKills = 2,
            WeaponScore = 99,
            WeaponFireCount = 500,
            WeaponHitCount = 250,
            WeaponDamageGiven = 1000,
            WeaponDamageTakenBy = 800
        };

        var result = _mapper.Map<CharacterDetailsLifetimeStats>(stats);

        result.Kills.Should().Be(10);
        result.Deaths.Should().Be(4);
        result.Headshots.Should().Be(3);
        result.PlayTime.Should().Be(600);
        result.VehicleKills.Should().Be(2);
        result.Score.Should().Be(99);
        result.FireCount.Should().Be(500);
        result.HitCount.Should().Be(250);
        result.DamageGiven.Should().Be(1000);
        result.DamageTakenBy.Should().Be(800);
    }

    [Fact]
    public void StatsByFaction_GroupsKillsAndKilledByPerFaction()
    {
        var stat = new CharacterStatByFaction
        {
            Profile = new Data.Models.Planetside.Profile { Name = "Infiltrator", ImageId = 5 },
            KillsVS = 1,
            KillsNC = 2,
            KillsTR = 3,
            KilledByVS = 4,
            KilledByNC = 5,
            KilledByTR = 6
        };

        var result = _mapper.Map<CharacterDetailsProfileStatByFaction>(stat);

        result.ProfileName.Should().Be("Infiltrator");
        result.ImageId.Should().Be(5);
        result.Kills.Vs.Should().Be(1);
        result.Kills.Nc.Should().Be(2);
        result.Kills.Tr.Should().Be(3);
        result.KilledBy.Vs.Should().Be(4);
        result.KilledBy.Nc.Should().Be(5);
        result.KilledBy.Tr.Should().Be(6);
    }

    [Fact]
    public void StatsByFaction_MissingValues_DefaultToZero()
    {
        var result = _mapper.Map<CharacterDetailsProfileStatByFaction>(new CharacterStatByFaction());

        result.Kills.Vs.Should().Be(0);
        result.KilledBy.Tr.Should().Be(0);
    }

    [Fact]
    public void StatsHistory_ParsesStoredJsonArrays()
    {
        var history = new CharacterStatHistory { Day = "[1,2,3]", Week = "[4,5]", Month = "[6]" };

        var result = _mapper.Map<CharacterDetailsStatsHistory>(history);

        result.Day.Should().Equal(1, 2, 3);
        result.Week.Should().Equal(4, 5);
        result.Month.Should().Equal(6);
    }

    [Fact]
    public void StatsHistory_ParsesLegacyIndentedJson()
    {
        var history = new CharacterStatHistory { Day = "[\n  1,\n  2\n]", Week = "[]", Month = "[]" };

        _mapper.Map<CharacterDetailsStatsHistory>(history).Day.Should().Equal(1, 2);
    }

    [Fact]
    public void OutfitMember_MapsOutfitDetails()
    {
        var created = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var member = new OutfitMember
        {
            OutfitId = "o1",
            Outfit = new Outfit { Name = "The Outfit", Alias = "TO", CreatedDate = created, MemberCount = 123 }
        };

        var result = _mapper.Map<CharacterDetailsOutfit>(member);

        result.Id.Should().Be("o1");
        result.Name.Should().Be("The Outfit");
        result.Alias.Should().Be("TO");
        result.CreatedDate.Should().Be(created);
        result.MemberCount.Should().Be(123);
    }

    [Fact]
    public void WeaponStat_MapsItemAndVehicleNames()
    {
        var stat = new CharacterWeaponStat
        {
            Item = new Item { Name = "Gauss SAW", ImageId = 77, ItemCategory = new ItemCategory { Name = "LMG" } },
            Vehicle = new Vehicle { Name = "Harasser", ImageId = 88 }
        };

        var result = _mapper.Map<CharacterDetailsWeaponStat>(stat);

        result.Name.Should().Be("Gauss SAW");
        result.Category.Should().Be("LMG");
        result.ImageId.Should().Be(77);
        result.VehicleName.Should().Be("Harasser");
        result.VehicleImageId.Should().Be(88);
    }
}
