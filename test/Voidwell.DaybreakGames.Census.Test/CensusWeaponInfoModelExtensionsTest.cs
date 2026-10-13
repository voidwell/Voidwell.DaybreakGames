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

        Assert.Equal("Gauss SAW", model.GetName());
        Assert.Equal("An LMG", model.GetDescription());
        Assert.Equal("LMG", model.GetCategory());
        Assert.Equal("Medium", model.GetRange());
    }

    [Fact]
    public void TextProperties_Missing_ReturnNull()
    {
        var model = new CensusWeaponInfoModel();

        Assert.Null(model.GetName());
        Assert.Null(model.GetDescription());
        Assert.Null(model.GetCategory());
        Assert.Null(model.GetRange());
    }

    [Fact]
    public void GetWeaponSpeed_UsesFirstFireMode()
    {
        Assert.Equal(500, Weapon(Mode(m => m.Speed = 500), Mode(m => m.Speed = 900)).GetWeaponSpeed());
    }

    [Fact]
    public void GetWeaponSpeed_NoFireModes_ReturnsNull()
    {
        Assert.Null(new CensusWeaponInfoModel().GetWeaponSpeed());
        Assert.Null(Weapon().GetWeaponSpeed());
    }

    [Fact]
    public void GetMinDamage_PrefersFireModeDamageMin()
    {
        var model = Weapon(Mode(m => m.DamageMin = 80), Mode(m => m.DamageMin = 60));
        model.Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { DamageMin = 10 };

        Assert.Equal(60, model.GetMinDamage());
    }

    [Fact]
    public void GetMinDamage_NoFireModeMin_FallsBackToDatasheet()
    {
        var model = Weapon(Mode(m => m.Damage = 100));
        model.Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { DamageMin = 25 };

        Assert.Equal(25, model.GetMinDamage());
    }

    [Fact]
    public void GetMinDamage_NoDatasheet_FallsBackToFireModeDamage()
    {
        Assert.Equal(90, Weapon(Mode(m => m.Damage = 100), Mode(m => m.Damage = 90)).GetMinDamage());
    }

    [Fact]
    public void GetMaxDamage_PrefersFireModeDamageMax()
    {
        var model = Weapon(Mode(m => m.DamageMax = 200), Mode(m => m.DamageMax = 150));
        model.Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { DamageMax = 10 };

        Assert.Equal(200, model.GetMaxDamage());
    }

    [Fact]
    public void GetMaxDamage_NoFireModeMax_FallsBackToDatasheet()
    {
        var model = Weapon(Mode(m => m.Damage = 100));
        model.Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { DamageMax = 175 };

        Assert.Equal(175, model.GetMaxDamage());
    }

    [Fact]
    public void GetMaxDamage_NoDatasheet_FallsBackToFireModeDamage()
    {
        Assert.Equal(100, Weapon(Mode(m => m.Damage = 100), Mode(m => m.Damage = 90)).GetMaxDamage());
    }

    [Fact]
    public void DamageRange_UsesExtremesAcrossFireModes()
    {
        var model = Weapon(
            Mode(m => { m.DamageMinRange = 10; m.DamageMaxRange = 50; }),
            Mode(m => { m.DamageMinRange = 5; m.DamageMaxRange = 70; }));

        Assert.Equal(5, model.GetMinDamageRange());
        Assert.Equal(70, model.GetMaxDamageRange());
    }

    [Fact]
    public void DamageRange_NoFireModes_ReturnsNull()
    {
        var model = new CensusWeaponInfoModel();

        Assert.Null(model.GetMinDamageRange());
        Assert.Null(model.GetMaxDamageRange());
    }

    [Fact]
    public void ReloadSpeed_SumsReloadAndChamberTime()
    {
        var model = Weapon(
            Mode(m => { m.ReloadTimeMs = 2000; m.ReloadChamberTimeMs = 500; }),
            Mode(m => { m.ReloadTimeMs = 3000; m.ReloadChamberTimeMs = 1000; }));

        Assert.Equal(2500, model.GetMinReloadSpeed());
        Assert.Equal(4000, model.GetMaxReloadSpeed());
    }

    [Fact]
    public void ReloadSpeed_NoChamberTime_FallsBackToDatasheet()
    {
        var model = Weapon(Mode(m => m.ReloadTimeMs = 2000));
        model.Datasheet = new CensusWeaponInfoModel.WeaponDatasheet { ReloadMsMin = 1800, ReloadMsMax = 2200 };

        Assert.Equal(1800, model.GetMinReloadSpeed());
        Assert.Equal(2200, model.GetMaxReloadSpeed());
    }

    [Fact]
    public void ReloadSpeed_NoChamberTimeAndNoDatasheet_FallsBackToReloadTime()
    {
        var model = Weapon(Mode(m => m.ReloadTimeMs = 2000), Mode(m => m.ReloadTimeMs = 3000));

        Assert.Equal(2000, model.GetMinReloadSpeed());
        Assert.Equal(3000, model.GetMaxReloadSpeed());
    }

    [Fact]
    public void GetFireModesOfType_FiltersByLowercasedTypeName()
    {
        var primary = Mode(m => m.Type = "primary");
        var secondary = Mode(m => m.Type = "secondary");
        var model = Weapon(primary, secondary);

        Assert.Equal(new[] { primary }, model.GetFireModesOfType(CensusWeaponInfoModelExtensions.FireModeType.Primary));
        Assert.Equal(new[] { secondary }, model.GetFireModesOfType(CensusWeaponInfoModelExtensions.FireModeType.Secondary));
    }

    [Fact]
    public void GetFireModesOfType_NoFireModes_ReturnsNull()
    {
        Assert.Null(new CensusWeaponInfoModel().GetFireModesOfType(CensusWeaponInfoModelExtensions.FireModeType.Primary));
    }

    [Fact]
    public void IndirectDamage_UsesExtremesAcrossFireModes()
    {
        var model = Weapon(
            Mode(m => { m.IndirectDamageMin = 10; m.IndirectDamageMax = 100; m.IndirectDamageMinRange = 1.5f; m.IndirectDamageMaxRange = 4f; m.DamageRadius = 3; }),
            Mode(m => { m.IndirectDamageMin = 20; m.IndirectDamageMax = 80; m.IndirectDamageMinRange = 0.5f; m.IndirectDamageMaxRange = 6f; m.DamageRadius = 8; }));

        Assert.Equal(10, model.GetIndirectMinDamage());
        Assert.Equal(100, model.GetIndirectMaxDamage());
        Assert.Equal(0.5f, model.GetIndirectMinDamageRange());
        Assert.Equal(6f, model.GetIndirectMaxDamageRange());
        Assert.Equal(8, model.GetDamageRadius());
    }

    [Fact]
    public void GetFireModeStateCoF_ReturnsConeForState()
    {
        var mode = Mode(m => m.States = new[]
        {
            new CensusWeaponInfoModel.WeaponFireModeState { PlayerState = "Standing", MinConeOfFire = 0.25f },
            new CensusWeaponInfoModel.WeaponFireModeState { PlayerState = "Crouching", MinConeOfFire = 0.1f }
        });

        Assert.Equal(0.25f, mode.GetFireModeStateCoF(CensusWeaponInfoModelExtensions.FireModeState.Standing));
        Assert.Equal(0.1f, mode.GetFireModeStateCoF(CensusWeaponInfoModelExtensions.FireModeState.Crouching));
        Assert.Null(mode.GetFireModeStateCoF(CensusWeaponInfoModelExtensions.FireModeState.Running));
    }

    [Fact]
    public void GetFireModeStateCoF_NoStates_ReturnsNull()
    {
        Assert.Null(Mode().GetFireModeStateCoF(CensusWeaponInfoModelExtensions.FireModeState.Standing));
    }

    [Fact]
    public void GetDefaultZoom_UsesFirstFireMode()
    {
        var modes = new[] { Mode(m => m.DefaultZoom = 4f), Mode(m => m.DefaultZoom = 8f) };

        Assert.Equal(4f, modes.GetDefaultZoom());
    }

    [Fact]
    public void GetDefaultZoom_Empty_ReturnsNull()
    {
        Assert.Null(Array.Empty<CensusWeaponInfoModel.WeaponFireMode>().GetDefaultZoom());
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

        Assert.Equal(new[] { "Semi-Auto", "Burst" }, modes.GetFireModeNames());
    }
}
