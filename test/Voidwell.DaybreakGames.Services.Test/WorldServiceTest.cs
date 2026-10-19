using Moq;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Test.Caching;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class WorldServiceTest
{
    private const int _activeWorldId = 1;
    private const int _legacyWorldId = 17;

    private readonly Mock<IWorldStore> _worldStore = new();

    private WorldService CreateSut()
    {
        _worldStore.Setup(a => a.GetAllWorlds()).ReturnsAsync(new[]
        {
            new World { Id = _activeWorldId, Name = "Osprey" },
            new World { Id = _legacyWorldId, Name = "Emerald", IsActive = false }
        });

        return new WorldService(_worldStore.Object, null!, null!, null!, TestCache.Create());
    }

    [Fact]
    public async Task GetWorlds_ExcludesInactiveByDefault()
    {
        var result = await CreateSut().GetWorlds();

        var world = Assert.Single(result);
        Assert.Equal(_activeWorldId, world.Id);
        Assert.True(world.IsActive);
    }

    [Fact]
    public async Task GetWorlds_IncludeInactive_ReturnsLegacyWorlds()
    {
        var result = (await CreateSut().GetWorlds(includeInactive: true)).ToList();

        Assert.Equal(2, result.Count);
        Assert.True(result.Single(a => a.Id == _activeWorldId).IsActive);
        Assert.False(result.Single(a => a.Id == _legacyWorldId).IsActive);
    }
}
