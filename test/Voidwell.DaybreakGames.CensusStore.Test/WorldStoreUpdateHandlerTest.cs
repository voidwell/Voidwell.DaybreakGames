using Moq;
using Voidwell.DaybreakGames.CensusStore.StoreUpdater;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Utils;
using Xunit;

namespace Voidwell.DaybreakGames.CensusStore.Test;

public class WorldStoreUpdateHandlerTest
{
    private readonly Mock<IWorldRepository> _worldRepository = new();
    private List<World> _upserted = new();

    private WorldStoreUpdateHandler CreateSut(params World[] storedWorlds)
    {
        _worldRepository.Setup(a => a.GetAllWorldsAsync()).ReturnsAsync(storedWorlds);
        _worldRepository.Setup(a => a.UpsertRangeAsync(It.IsAny<IEnumerable<World>>()))
            .Callback<IEnumerable<World>>(a => _upserted = a.ToList())
            .Returns(Task.CompletedTask);

        return new WorldStoreUpdateHandler(_worldRepository.Object);
    }

    [Fact]
    public async Task UpdateAsync_NewWorld_IsAddedActive()
    {
        var sut = CreateSut();

        await sut.UpdateAsync(new[] { new World { Id = 1, Name = "Osprey", IsActive = false } });

        var world = Assert.Single(_upserted);
        Assert.Equal(1, world.Id);
        Assert.True(world.IsActive);
    }

    [Fact]
    public async Task UpdateAsync_ExistingWorld_KeepsActiveState()
    {
        var sut = CreateSut(
            new World { Id = 1, Name = "Old name", IsActive = true },
            new World { Id = 25, Name = "Briggs", IsActive = false });

        await sut.UpdateAsync(new[]
        {
            new World { Id = 1, Name = "Osprey" },
            new World { Id = 25, Name = "Briggs" }
        });

        Assert.Equal(2, _upserted.Count);
        Assert.Equal("Osprey", _upserted.Single(a => a.Id == 1).Name);
        Assert.True(_upserted.Single(a => a.Id == 1).IsActive);
        Assert.False(_upserted.Single(a => a.Id == 25).IsActive);
    }

    [Fact]
    public async Task UpdateAsync_WorldMissingFromCensus_IsDeactivated()
    {
        var sut = CreateSut(
            new World { Id = 1, Name = "Osprey", IsActive = true },
            new World { Id = 17, Name = "Emerald", IsActive = true },
            new World { Id = 13, Name = "Cobalt", IsActive = false });

        await sut.UpdateAsync(new[] { new World { Id = 1, Name = "Osprey" } });

        Assert.Equal(new[] { 1, 17 }, _upserted.Select(a => a.Id).Order());
        Assert.False(_upserted.Single(a => a.Id == 17).IsActive);
    }

    [Fact]
    public async Task UpdateAsync_EmptyCensusResponse_ChangesNothing()
    {
        var sut = CreateSut(new World { Id = 1, Name = "Osprey", IsActive = true });

        await sut.UpdateAsync(Array.Empty<World>());

        _worldRepository.Verify(a => a.UpsertRangeAsync(It.IsAny<IEnumerable<World>>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_PlatformMissingFromCensus_KeepsItsWorlds()
    {
        var sut = CreateSut(
            new World { Id = 1, Name = "Osprey", IsActive = true, Platform = Platform.PC },
            new World { Id = 10, Name = "Miller", IsActive = true, Platform = Platform.PC },
            new World { Id = 1000, Name = "Genudine", IsActive = true, Platform = Platform.PS4US });

        // PS4 US returned nothing (disabled or failed), PC dropped Miller
        await sut.UpdateAsync(new[] { new World { Id = 1, Name = "Osprey", Platform = Platform.PC } });

        Assert.Equal(new[] { 1, 10 }, _upserted.Select(a => a.Id).Order());
        Assert.False(_upserted.Single(a => a.Id == 10).IsActive);
        Assert.DoesNotContain(_upserted, a => a.Id == 1000);
    }
}
