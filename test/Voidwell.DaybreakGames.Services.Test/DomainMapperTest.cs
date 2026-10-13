using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Services.Mappers;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class DomainMapperTest
{
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

        var details = DomainMapper.MapCharacterDetails(character);

        Assert.Equal("Vanu Sovereignty", details.Faction);
        Assert.Equal(12, details.FactionImageId);
        Assert.Equal("Ace", details.Title);
        Assert.Equal("Emerald", details.World);
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

        var result = Assert.Single(DomainMapper.MapCharacterDetails(character).VehicleStats);

        Assert.Equal(5, result.VehicleId);
        Assert.Equal(7, result.Kills);
        Assert.Equal(3, result.Deaths);
        Assert.Equal(2, result.PilotKills);
        Assert.Equal(1, result.PilotVehicleKills);
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

        var result = DomainMapper.MapLifetimeStats(stats);

        Assert.Equal(10, result.Kills);
        Assert.Equal(4, result.Deaths);
        Assert.Equal(3, result.Headshots);
        Assert.Equal(600, result.PlayTime);
        Assert.Equal(2, result.VehicleKills);
        Assert.Equal(99, result.Score);
        Assert.Equal(500, result.FireCount);
        Assert.Equal(250, result.HitCount);
        Assert.Equal(1000, result.DamageGiven);
        Assert.Equal(800, result.DamageTakenBy);
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

        var result = DomainMapper.MapProfileStatByFaction(stat);

        Assert.Equal("Infiltrator", result.ProfileName);
        Assert.Equal(5, result.ImageId);
        Assert.Equal(1, result.Kills.Vs);
        Assert.Equal(2, result.Kills.Nc);
        Assert.Equal(3, result.Kills.Tr);
        Assert.Equal(4, result.KilledBy.Vs);
        Assert.Equal(5, result.KilledBy.Nc);
        Assert.Equal(6, result.KilledBy.Tr);
    }

    [Fact]
    public void StatsByFaction_MissingValues_DefaultToZero()
    {
        var result = DomainMapper.MapProfileStatByFaction(new CharacterStatByFaction());

        Assert.Equal(0, result.Kills.Vs);
        Assert.Equal(0, result.KilledBy.Tr);
    }

    [Fact]
    public void StatsHistory_ParsesStoredJsonArrays()
    {
        var history = new CharacterStatHistory { Day = "[1,2,3]", Week = "[4,5]", Month = "[6]" };

        var result = DomainMapper.MapStatsHistory(history);

        Assert.Equal(new[] { 1, 2, 3 }, result.Day);
        Assert.Equal(new[] { 4, 5 }, result.Week);
        Assert.Equal(new[] { 6 }, result.Month);
    }

    [Fact]
    public void StatsHistory_ParsesLegacyIndentedJson()
    {
        var history = new CharacterStatHistory { Day = "[\n  1,\n  2\n]", Week = "[]", Month = "[]" };

        Assert.Equal(new[] { 1, 2 }, DomainMapper.MapStatsHistory(history).Day);
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

        var result = DomainMapper.MapOutfit(member);

        Assert.Equal("o1", result.Id);
        Assert.Equal("The Outfit", result.Name);
        Assert.Equal("TO", result.Alias);
        Assert.Equal(created, result.CreatedDate);
        Assert.Equal(123, result.MemberCount);
    }

    [Fact]
    public void WeaponStat_MapsItemAndVehicleNames()
    {
        var stat = new CharacterWeaponStat
        {
            Item = new Item { Name = "Gauss SAW", ImageId = 77, ItemCategory = new ItemCategory { Name = "LMG" } },
            Vehicle = new Vehicle { Name = "Harasser", ImageId = 88 }
        };

        var result = DomainMapper.MapWeaponStat(stat);

        Assert.Equal("Gauss SAW", result.Name);
        Assert.Equal("LMG", result.Category);
        Assert.Equal(77, result.ImageId);
        Assert.Equal("Harasser", result.VehicleName);
        Assert.Equal(88, result.VehicleImageId);
    }
}
