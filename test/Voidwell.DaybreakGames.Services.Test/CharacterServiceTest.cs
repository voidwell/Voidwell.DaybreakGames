using Moq;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using Voidwell.DaybreakGames.Test.Caching;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class CharacterServiceTest
{
    private const long _characterId = 5428779225270452929;

    private readonly Mock<ICharacterStore> _characterStore = new();
    private readonly Mock<IWeaponAggregateService> _weaponAggregateService = new();
    private readonly Mock<IWeaponService> _weaponService = new();

    private CharacterService CreateSut(Character character)
    {
        _characterStore.Setup(a => a.GetCharacterDetailsAsync(_characterId)).ReturnsAsync(character);
        _weaponService.Setup(a => a.GetAllSanctionedWeaponIds()).ReturnsAsync(Array.Empty<int>());
        _weaponAggregateService.Setup(a => a.GetAggregates(It.IsAny<IEnumerable<int>>())).ReturnsAsync(new Dictionary<string, WeaponAggregate>());

        return new CharacterService(_characterStore.Object, TestCache.Create(), _weaponAggregateService.Object, null!, _weaponService.Object);
    }

    [Fact]
    public async Task GetCharacterDetails_WithoutLifetimeStats_ReturnsDetails()
    {
        // A character that has not had its lifetime stats stored yet comes back from the left join with null LifetimeStats.
        var character = new Character { Id = _characterId, Name = "NewCharacter", WeaponStats = [], LifetimeStats = null };

        var result = await CreateSut(character).GetCharacterDetails(_characterId);

        Assert.NotNull(result);
        Assert.Null(result.LifetimeStats);
        Assert.NotNull(result.InfantryStats);
        Assert.Null(result.InfantryStats.KDRPadding);
    }

    [Fact]
    public async Task GetCharacterDetails_WithLifetimeStats_CalculatesKdrPadding()
    {
        var character = new Character
        {
            Id = _characterId,
            Name = "Veteran",
            WeaponStats = [],
            LifetimeStats = new CharacterLifetimeStat { CharacterId = _characterId, WeaponKills = 300, WeaponDeaths = 100 }
        };

        var result = await CreateSut(character).GetCharacterDetails(_characterId);

        Assert.NotNull(result?.InfantryStats);
        Assert.Equal(3.0, result.InfantryStats.KDRPadding);
    }
}
