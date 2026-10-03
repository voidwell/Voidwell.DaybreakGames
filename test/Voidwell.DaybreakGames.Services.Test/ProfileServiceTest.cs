using FluentAssertions;
using Moq;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Services.Planetside;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class ProfileServiceTest
{
    private readonly Mock<IProfileStore> _profileStore = new();
    private readonly Mock<ILoadoutStore> _loadoutStore = new();

    private ProfileService CreateSut()
    {
        _profileStore.Setup(a => a.GetAllProfilesAsync()).ReturnsAsync(new[]
        {
            new Profile { Id = 1, Name = "Infiltrator" },
            new Profile { Id = 3, Name = "Light Assault" }
        });
        _loadoutStore.Setup(a => a.GetAllLoadoutsAsync()).ReturnsAsync(new[]
        {
            new Loadout { Id = 10, ProfileId = 1 },
            new Loadout { Id = 11, ProfileId = 3 },
            new Loadout { Id = 12, ProfileId = 99 }
        });

        return new ProfileService(_profileStore.Object, _loadoutStore.Object);
    }

    [Fact]
    public async Task GetProfileFromLoadoutIdAsync_ReturnsProfileOfLoadout()
    {
        using var sut = CreateSut();

        (await sut.GetProfileFromLoadoutIdAsync(10)).Name.Should().Be("Infiltrator");
        (await sut.GetProfileFromLoadoutIdAsync(11)).Name.Should().Be("Light Assault");
    }

    [Fact]
    public async Task GetProfileFromLoadoutIdAsync_UnknownLoadout_ReturnsNull()
    {
        using var sut = CreateSut();

        (await sut.GetProfileFromLoadoutIdAsync(404)).Should().BeNull();
    }

    [Fact]
    public async Task GetProfileFromLoadoutIdAsync_LoadoutWithUnknownProfile_ReturnsNull()
    {
        using var sut = CreateSut();

        (await sut.GetProfileFromLoadoutIdAsync(12)).Should().BeNull();
    }

    [Fact]
    public async Task GetProfileFromLoadoutIdAsync_BuildsMapOnlyOnce()
    {
        using var sut = CreateSut();

        await sut.GetProfileFromLoadoutIdAsync(10);
        await sut.GetProfileFromLoadoutIdAsync(11);
        await sut.GetProfileFromLoadoutIdAsync(10);

        _loadoutStore.Verify(a => a.GetAllLoadoutsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetProfileFromLoadoutIdAsync_ConcurrentFirstCalls_BuildMapOnce()
    {
        using var sut = CreateSut();

        await Task.WhenAll(Enumerable.Range(0, 10).Select(_ => sut.GetProfileFromLoadoutIdAsync(10)));

        _loadoutStore.Verify(a => a.GetAllLoadoutsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllProfiles_DelegatesToStore()
    {
        using var sut = CreateSut();

        (await sut.GetAllProfiles()).Should().HaveCount(2);
    }
}
