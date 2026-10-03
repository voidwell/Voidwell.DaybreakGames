using FluentAssertions;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Models.Extensions;
using Xunit;

namespace Voidwell.DaybreakGames.Census.Test;

public class CensusWeaponInfoModelExtensionsTest
{
    private static CensusWeaponInfoModel.WeaponFireMode Mode(Action<CensusWeaponInfoModel.WeaponFireMode> configure = null)
    {
        var mode = new CensusWeaponInfoModel.WeaponFireMode();
        configure?.Invoke(mode);
        return mode;
    }

    private static CensusWeaponInfoModel Weapon(params CensusWeaponInfoModel.WeaponFireMode[] modes)
    {
        return new CensusWeaponInfoModel { FireMode = modes };
    }

    [Fact]
    public void TextProperties_ReturnEnglishText()
    {
        var model = new CensusWeaponInfoModel
        {
            Name = new MultiLanguageString { English = "Gauss SAW" },
            Description = new MultiLanguageString { English = "An LMG" },
            Category = new CensusWeaponInfoModel.WeaponCategory { Name = new MultiLanguageString { English = "LMG" } },
            Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { Range = new MultiLanguageString { English = "Medium" } }
        };

        model.GetName().Should().Be("Gauss SAW");
        model.GetDescription().Should().Be("An LMG");
        model.GetCategory().Should().Be("LMG");
        model.GetRange().Should().Be("Medium");
    }

    [Fact]
    public void TextProperties_Missing_ReturnNull()
    {
        var model = new CensusWeaponInfoModel();

        model.GetName().Should().BeNull();
        model.GetDescription().Should().BeNull();
        model.GetCategory().Should().BeNull();
        model.GetRange().Should().BeNull();
    }

    [Fact]
    public void GetWeaponSpeed_UsesFirstFireMode()
    {
        Weapon(Mode(m => m.Speed = 500), Mode(m => m.Speed = 900)).GetWeaponSpeed().Should().Be(500);
    }

    [Fact]
    public void GetWeaponSpeed_NoFireModes_ReturnsNull()
    {
        new CensusWeaponInfoModel().GetWeaponSpeed().Should().BeNull();
        Weapon().GetWeaponSpeed().Should().BeNull();
    }

    [Fact]
    public void GetMinDamage_PrefersFireModeDamageMin()
    {
        var model = Weapon(Mode(m => m.DamageMin = 80), Mode(m => m.DamageMin = 60));
        model.Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { DamageMin = 10 };

        model.GetMinDamage().Should().Be(60);
    }

    [Fact]
    public void GetMinDamage_NoFireModeMin_FallsBackToDatasheet()
    {
        var model = Weapon(Mode(m => m.Damage = 100));
        model.Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { DamageMin = 25 };

        model.GetMinDamage().Should().Be(25);
    }

    [Fact]
    public void GetMinDamage_NoDatasheet_FallsBackToFireModeDamage()
    {
        Weapon(Mode(m => m.Damage = 100), Mode(m => m.Damage = 90)).GetMinDamage().Should().Be(90);
    }

    [Fact]
    public void GetMaxDamage_PrefersFireModeDamageMax()
    {
        var model = Weapon(Mode(m => m.DamageMax = 200), Mode(m => m.DamageMax = 150));
        model.Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { DamageMax = 10 };

        model.GetMaxDamage().Should().Be(200);
    }

    [Fact]
    public void GetMaxDamage_NoFireModeMax_FallsBackToDatasheet()
    {
        var model = Weapon(Mode(m => m.Damage = 100));
        model.Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { DamageMax = 175 };

        model.GetMaxDamage().Should().Be(175);
    }

    [Fact]
    public void GetMaxDamage_NoDatasheet_FallsBackToFireModeDamage()
    {
        Weapon(Mode(m => m.Damage = 100), Mode(m => m.Damage = 90)).GetMaxDamage().Should().Be(100);
    }

    [Fact]
    public void DamageRange_UsesExtremesAcrossFireModes()
    {
        var model = Weapon(
            Mode(m => { m.DamageMinRange = 10; m.DamageMaxRange = 50; }),
            Mode(m => { m.DamageMinRange = 5; m.DamageMaxRange = 70; }));

        model.GetMinDamageRange().Should().Be(5);
        model.GetMaxDamageRange().Should().Be(70);
    }

    [Fact]
    public void DamageRange_NoFireModes_ReturnsNull()
    {
        var model = new CensusWeaponInfoModel();

        model.GetMinDamageRange().Should().BeNull();
        model.GetMaxDamageRange().Should().BeNull();
    }

    [Fact]
    public void ReloadSpeed_SumsReloadAndChamberTime()
    {
        var model = Weapon(
            Mode(m => { m.ReloadTimeMs = 2000; m.ReloadChamberTimeMs = 500; }),
            Mode(m => { m.ReloadTimeMs = 3000; m.ReloadChamberTimeMs = 1000; }));

        model.GetMinReloadSpeed().Should().Be(2500);
        model.GetMaxReloadSpeed().Should().Be(4000);
    }

    [Fact]
    public void ReloadSpeed_NoChamberTime_FallsBackToDatasheet()
    {
        var model = Weapon(Mode(m => m.ReloadTimeMs = 2000));
        model.Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { ReloadMsMin = 1800, ReloadMsMax = 2200 };

        model.GetMinReloadSpeed().Should().Be(1800);
        model.GetMaxReloadSpeed().Should().Be(2200);
    }

    [Fact]
    public void ReloadSpeed_NoChamberTimeAndNoDatasheet_FallsBackToReloadTime()
    {
        var model = Weapon(Mode(m => m.ReloadTimeMs = 2000), Mode(m => m.ReloadTimeMs = 3000));

        model.GetMinReloadSpeed().Should().Be(2000);
        model.GetMaxReloadSpeed().Should().Be(3000);
    }

    [Fact]
    public void GetFireModesOfType_FiltersByLowercasedTypeName()
    {
        var primary = Mode(m => m.Type = "primary");
        var secondary = Mode(m => m.Type = "secondary");
        var model = Weapon(primary, secondary);

        model.GetFireModesOfType(CensusWeaponInfoModelExtensions.FireModeType.Primary).Should().Equal(primary);
        model.GetFireModesOfType(CensusWeaponInfoModelExtensions.FireModeType.Secondary).Should().Equal(secondary);
    }

    [Fact]
    public void GetFireModesOfType_NoFireModes_ReturnsNull()
    {
        new CensusWeaponInfoModel().GetFireModesOfType(CensusWeaponInfoModelExtensions.FireModeType.Primary).Should().BeNull();
    }

    [Fact]
    public void IndirectDamage_UsesExtremesAcrossFireModes()
    {
        var model = Weapon(
            Mode(m => { m.IndirectDamageMin = 10; m.IndirectDamageMax = 100; m.IndirectDamageMinRange = 1.5f; m.IndirectDamageMaxRange = 4f; m.DamageRadius = 3; }),
            Mode(m => { m.IndirectDamageMin = 20; m.IndirectDamageMax = 80; m.IndirectDamageMinRange = 0.5f; m.IndirectDamageMaxRange = 6f; m.DamageRadius = 8; }));

        model.GetIndirectMinDamage().Should().Be(10);
        model.GetIndirectMaxDamage().Should().Be(100);
        model.GetIndirectMinDamageRange().Should().Be(0.5f);
        model.GetIndirectMaxDamageRange().Should().Be(6f);
        model.GetDamageRadius().Should().Be(8);
    }

    [Fact]
    public void GetFireModeStateCoF_ReturnsConeForState()
    {
        var mode = Mode(m => m.States = new[]
        {
            new CensusWeaponInfoModel.WeaponFireModeState { PlayerState = "Standing", MinConeOfFire = 0.25f },
            new CensusWeaponInfoModel.WeaponFireModeState { PlayerState = "Crouching", MinConeOfFire = 0.1f }
        });

        mode.GetFireModeStateCoF(CensusWeaponInfoModelExtensions.FireModeState.Standing).Should().Be(0.25f);
        mode.GetFireModeStateCoF(CensusWeaponInfoModelExtensions.FireModeState.Crouching).Should().Be(0.1f);
        mode.GetFireModeStateCoF(CensusWeaponInfoModelExtensions.FireModeState.Running).Should().BeNull();
    }

    [Fact]
    public void GetFireModeStateCoF_NoStates_ReturnsNull()
    {
        Mode().GetFireModeStateCoF(CensusWeaponInfoModelExtensions.FireModeState.Standing).Should().BeNull();
    }

    [Fact]
    public void GetDefaultZoom_UsesFirstFireMode()
    {
        var modes = new[] { Mode(m => m.DefaultZoom = 4f), Mode(m => m.DefaultZoom = 8f) };

        modes.GetDefaultZoom().Should().Be(4f);
    }

    [Fact]
    public void GetDefaultZoom_Empty_ReturnsNull()
    {
        Array.Empty<CensusWeaponInfoModel.WeaponFireMode>().GetDefaultZoom().Should().BeNull();
    }

    [Fact]
    public void GetFireModeNames_ReturnsDistinctEnglishDescriptions()
    {
        var modes = new[]
        {
            Mode(m => m.Description = new MultiLanguageString { English = "Semi-Auto" }),
            Mode(m => m.Description = new MultiLanguageString { English = "Semi-Auto" }),
            Mode(m => m.Description = new MultiLanguageString { English = "Burst" })
        };

        modes.GetFireModeNames().Should().Equal("Semi-Auto", "Burst");
    }
}
