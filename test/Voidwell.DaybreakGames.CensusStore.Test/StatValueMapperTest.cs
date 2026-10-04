using FluentAssertions;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Xunit;

namespace Voidwell.DaybreakGames.CensusStore.Test;

public class StatValueMapperTest
{
    [Theory]
    [InlineData("achievement_count", "AchievementCount")]
    [InlineData("assist_count", "AssistCount")]
    [InlineData("facility_defended_count", "FacilityDefendedCount")]
    [InlineData("medal_count", "MedalCount")]
    [InlineData("skill_points", "SkillPoints")]
    [InlineData("weapon_deaths", "WeaponDeaths")]
    [InlineData("weapon_fire_count", "WeaponFireCount")]
    [InlineData("weapon_hit_count", "WeaponHitCount")]
    [InlineData("weapon_play_time", "WeaponPlayTime")]
    [InlineData("weapon_score", "WeaponScore")]
    [InlineData("domination_count", "DominationCount")]
    [InlineData("facility_capture_count", "FacilityCaptureCount")]
    [InlineData("revenge_count", "RevengeCount")]
    [InlineData("weapon_damage_given", "WeaponDamageGiven")]
    [InlineData("weapon_damage_taken_by", "WeaponDamageTakenBy")]
    [InlineData("weapon_headshots", "WeaponHeadshots")]
    [InlineData("weapon_kills", "WeaponKills")]
    [InlineData("weapon_vehicle_kills", "WeaponVehicleKills")]
    public void AssignStatValue_CharacterLifetimeStat_SetsMappedProperty(string statName, string propertyName)
    {
        var model = new CharacterLifetimeStat();

        StatValueMapper.AssignStatValue(ref model, statName, 7);

        typeof(CharacterLifetimeStat).GetProperty(propertyName)!.GetValue(model).Should().Be(7);
    }

    [Fact]
    public void AssignStatValue_CharacterLifetimeStat_UnknownStat_ChangesNothing()
    {
        var model = new CharacterLifetimeStat();

        StatValueMapper.AssignStatValue(ref model, "not_a_stat", 7);

        model.Should().BeEquivalentTo(new CharacterLifetimeStat());
    }

    [Theory]
    [InlineData("deaths", "Deaths")]
    [InlineData("fire_count", "FireCount")]
    [InlineData("hit_count", "HitCount")]
    [InlineData("play_time", "PlayTime")]
    [InlineData("score", "Score")]
    [InlineData("killed_by", "KilledBy")]
    [InlineData("kills", "Kills")]
    public void AssignStatValue_CharacterStat_SetsMappedProperty(string statName, string propertyName)
    {
        var model = new CharacterStat();

        StatValueMapper.AssignStatValue(ref model, statName, 7);

        typeof(CharacterStat).GetProperty(propertyName)!.GetValue(model).Should().Be(7);
    }

    [Fact]
    public void AssignStatValue_CharacterStat_UnknownStat_ChangesNothing()
    {
        var model = new CharacterStat();

        StatValueMapper.AssignStatValue(ref model, "not_a_stat", 7);

        model.Should().BeEquivalentTo(new CharacterStat());
    }

    [Theory]
    [InlineData("domination_count", "DominationCount")]
    [InlineData("facility_capture_count", "FacilityCaptureCount")]
    [InlineData("revenge_count", "RevengeCount")]
    [InlineData("weapon_damage_given", "WeaponDamageGiven")]
    [InlineData("weapon_damage_taken_by", "WeaponDamageTakenBy")]
    [InlineData("weapon_headshots", "WeaponHeadshots")]
    [InlineData("weapon_killed_by", "WeaponKilledBy")]
    [InlineData("weapon_kills", "WeaponKills")]
    [InlineData("weapon_vehicle_kills", "WeaponVehicleKills")]
    public void AssignStatValue_CharacterLifetimeStatByFaction_SetsEachFactionProperty(string statName, string propertyPrefix)
    {
        var model = new CharacterLifetimeStatByFaction();

        StatValueMapper.AssignStatValue(ref model, statName, 1, 2, 3);

        var type = typeof(CharacterLifetimeStatByFaction);
        type.GetProperty(propertyPrefix + "VS")!.GetValue(model).Should().Be(1);
        type.GetProperty(propertyPrefix + "NC")!.GetValue(model).Should().Be(2);
        type.GetProperty(propertyPrefix + "TR")!.GetValue(model).Should().Be(3);
    }

    [Fact]
    public void AssignStatValue_CharacterLifetimeStatByFaction_UnknownStat_ChangesNothing()
    {
        var model = new CharacterLifetimeStatByFaction();

        StatValueMapper.AssignStatValue(ref model, "not_a_stat", 1, 2, 3);

        model.Should().BeEquivalentTo(new CharacterLifetimeStatByFaction());
    }

    [Theory]
    [InlineData("killed_by", "KilledBy")]
    [InlineData("kills", "Kills")]
    public void AssignStatValue_CharacterStatByFaction_SetsEachFactionProperty(string statName, string propertyPrefix)
    {
        var model = new CharacterStatByFaction();

        StatValueMapper.AssignStatValue(ref model, statName, 1, 2, 3);

        var type = typeof(CharacterStatByFaction);
        type.GetProperty(propertyPrefix + "VS")!.GetValue(model).Should().Be(1);
        type.GetProperty(propertyPrefix + "NC")!.GetValue(model).Should().Be(2);
        type.GetProperty(propertyPrefix + "TR")!.GetValue(model).Should().Be(3);
    }

    [Fact]
    public void AssignStatValue_CharacterStatByFaction_UnknownStat_ChangesNothing()
    {
        var model = new CharacterStatByFaction();

        StatValueMapper.AssignStatValue(ref model, "not_a_stat", 1, 2, 3);

        model.Should().BeEquivalentTo(new CharacterStatByFaction());
    }

    [Theory]
    [InlineData("weapon_deaths", "Deaths")]
    [InlineData("weapon_fire_count", "FireCount")]
    [InlineData("weapon_hit_count", "HitCount")]
    [InlineData("weapon_play_time", "PlayTime")]
    [InlineData("weapon_score", "Score")]
    [InlineData("weapon_damage_given", "DamageGiven")]
    [InlineData("weapon_headshots", "Headshots")]
    [InlineData("weapon_killed_by", "KilledBy")]
    [InlineData("weapon_kills", "Kills")]
    [InlineData("weapon_vehicle_kills", "VehicleKills")]
    [InlineData("weapon_damage_taken_by", "DamageTakenBy")]
    public void AssignStatValue_CharacterWeaponStat_SetsMappedProperty(string statName, string propertyName)
    {
        var model = new CharacterWeaponStat();

        StatValueMapper.AssignStatValue(ref model, statName, 7);

        typeof(CharacterWeaponStat).GetProperty(propertyName)!.GetValue(model).Should().Be(7);
    }

    [Fact]
    public void AssignStatValue_CharacterWeaponStat_UnknownStat_ChangesNothing()
    {
        var model = new CharacterWeaponStat();

        StatValueMapper.AssignStatValue(ref model, "not_a_stat", 7);

        model.Should().BeEquivalentTo(new CharacterWeaponStat());
    }

    [Theory]
    [InlineData("weapon_damage_taken_by", "DamageTakenBy")]
    [InlineData("weapon_damage_given", "DamageGiven")]
    [InlineData("weapon_headshots", "Headshots")]
    [InlineData("weapon_killed_by", "KilledBy")]
    [InlineData("weapon_kills", "Kills")]
    [InlineData("weapon_vehicle_kills", "VehicleKills")]
    public void AssignStatValue_CharacterWeaponStatByFaction_SetsEachFactionProperty(string statName, string propertyPrefix)
    {
        var model = new CharacterWeaponStatByFaction();

        StatValueMapper.AssignStatValue(ref model, statName, 1, 2, 3);

        var type = typeof(CharacterWeaponStatByFaction);
        type.GetProperty(propertyPrefix + "VS")!.GetValue(model).Should().Be(1);
        type.GetProperty(propertyPrefix + "NC")!.GetValue(model).Should().Be(2);
        type.GetProperty(propertyPrefix + "TR")!.GetValue(model).Should().Be(3);
    }

    [Fact]
    public void AssignStatValue_CharacterWeaponStatByFaction_UnknownStat_ChangesNothing()
    {
        var model = new CharacterWeaponStatByFaction();

        StatValueMapper.AssignStatValue(ref model, "not_a_stat", 1, 2, 3);

        model.Should().BeEquivalentTo(new CharacterWeaponStatByFaction());
    }
}
