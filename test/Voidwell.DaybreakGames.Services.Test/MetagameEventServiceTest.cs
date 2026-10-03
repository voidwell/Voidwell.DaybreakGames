using FluentAssertions;
using Moq;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Services.Planetside;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class MetagameEventServiceTest
{
    private readonly Mock<IMetagameEventStore> _store = new();

    [Fact]
    public async Task GetMetagameEvent_UnknownEvent_ReturnsNull()
    {
        _store.Setup(a => a.GetMetagameEventCategoryAsync(1)).ReturnsAsync((MetagameEventCategory)null);
        _store.Setup(a => a.GetMetagameCategoryZoneIdAsync(1)).ReturnsAsync((int?)null);
        var sut = new MetagameEventService(_store.Object);

        (await sut.GetMetagameEvent(1)).Should().BeNull();
    }

    [Fact]
    public async Task GetMetagameEvent_MapsCategoryAndZone()
    {
        _store.Setup(a => a.GetMetagameEventCategoryAsync(123)).ReturnsAsync(new MetagameEventCategory { Id = 123, Name = "Indar Alert", Description = "Capture it", Type = 1 });
        _store.Setup(a => a.GetMetagameCategoryZoneIdAsync(123)).ReturnsAsync(2);
        var sut = new MetagameEventService(_store.Object);

        var result = await sut.GetMetagameEvent(123);

        result.Id.Should().Be(123);
        result.TypeId.Should().Be(1);
        result.Name.Should().Be("Indar Alert");
        result.Description.Should().Be("Capture it");
        result.ZoneId.Should().Be(2);
    }

    [Theory]
    [InlineData(1, 80)]
    [InlineData(2, 45)]
    [InlineData(5, 5)]
    [InlineData(8, 45)]
    [InlineData(9, 90)]
    [InlineData(10, 25)]
    public async Task GetMetagameEvent_UsesDurationForEventType(int type, int expectedMinutes)
    {
        _store.Setup(a => a.GetMetagameEventCategoryAsync(1)).ReturnsAsync(new MetagameEventCategory { Id = 1, Type = type });
        _store.Setup(a => a.GetMetagameCategoryZoneIdAsync(1)).ReturnsAsync((int?)null);
        var sut = new MetagameEventService(_store.Object);

        var result = await sut.GetMetagameEvent(1);

        result.Duration.Should().Be(TimeSpan.FromMinutes(expectedMinutes));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(3)]
    [InlineData(99)]
    public async Task GetMetagameEvent_UnknownType_DefaultsToFortyFiveMinutes(int? type)
    {
        _store.Setup(a => a.GetMetagameEventCategoryAsync(1)).ReturnsAsync(new MetagameEventCategory { Id = 1, Type = type });
        _store.Setup(a => a.GetMetagameCategoryZoneIdAsync(1)).ReturnsAsync((int?)null);
        var sut = new MetagameEventService(_store.Object);

        var result = await sut.GetMetagameEvent(1);

        result.Duration.Should().Be(TimeSpan.FromMinutes(45));
        result.TypeId.Should().Be(type.GetValueOrDefault());
    }
}
