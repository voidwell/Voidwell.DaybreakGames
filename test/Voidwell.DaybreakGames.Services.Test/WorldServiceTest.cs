using Moq;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Test.Caching;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class WorldServiceTest
{
    private const int _legacyWorldId = 17;

    private readonly Mock<IWorldStore> _worldStore = new();

    private WorldService CreateSut()
    {
        _worldStore.Setup(a => a.GetAllWorlds()).ReturnsAsync(new[]
        {
            new World { Id = ActiveWorlds.Osprey, Name = "Osprey" },
            new World { Id = _legacyWorldId, Name = "Emerald" }
        });

        return new WorldService(_worldStore.Object, null!, null!, null!, TestCache.Create());
    }

    [Fact]
    public async Task GetWorlds_ExcludesInactiveByDefault()
    {
        var result = await CreateSut().GetWorlds();

        var world = Assert.Single(result);
        Assert.Equal(ActiveWorlds.Osprey, world.Id);
        Assert.False(world.IsInactive);
    }

    [Fact]
    public async Task GetWorlds_IncludeInactive_FlagsLegacyWorlds()
    {
        var result = (await CreateSut().GetWorlds(includeInactive: true)).ToList();

        Assert.Equal(2, result.Count);
        Assert.False(result.Single(a => a.Id == ActiveWorlds.Osprey).IsInactive);
        Assert.True(result.Single(a => a.Id == _legacyWorldId).IsInactive);
    }
}
