using Moq;
using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Test.Caching;
using Voidwell.DaybreakGames.Utils;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class WorldServiceTest
{
    private const int _activeWorldId = 1;
    private const int _legacyWorldId = 17;
    private const int _ps4WorldId = 1000;

    private readonly Mock<IWorldStore> _worldStore = new();
    private readonly CensusContext _censusContext = new();

    private WorldService CreateSut()
    {
        _worldStore.Setup(a => a.GetAllWorlds()).ReturnsAsync(new[]
        {
            new World { Id = _activeWorldId, Name = "Osprey" },
            new World { Id = _legacyWorldId, Name = "Emerald", IsActive = false },
            new World { Id = _ps4WorldId, Name = "Genudine", Platform = Platform.PS4US }
        });

        return new WorldService(_worldStore.Object, null!, null!, null!, TestCache.Create(), _censusContext);
    }

    [Fact]
    public async Task GetWorlds_ExcludesInactiveByDefault()
    {
        var result = await CreateSut().GetWorlds();

        Assert.Equal(new[] { _activeWorldId, _ps4WorldId }, result.Select(a => a.Id));
        Assert.All(result, a => Assert.True(a.IsActive));
    }

    [Fact]
    public async Task GetWorlds_IncludeInactive_ReturnsLegacyWorlds()
    {
        var result = (await CreateSut().GetWorlds(includeInactive: true)).ToList();

        Assert.Equal(3, result.Count);
        Assert.True(result.Single(a => a.Id == _activeWorldId).IsActive);
        Assert.False(result.Single(a => a.Id == _legacyWorldId).IsActive);
    }

    [Fact]
    public async Task GetWorlds_PlatformSet_ReturnsOnlyThatPlatform()
    {
        var sut = CreateSut();

        IEnumerable<World> result;
        using (_censusContext.Use(Platform.PS4US))
        {
            result = await sut.GetWorlds(includeInactive: true);
        }

        var world = Assert.Single(result);
        Assert.Equal(_ps4WorldId, world.Id);
    }
}
