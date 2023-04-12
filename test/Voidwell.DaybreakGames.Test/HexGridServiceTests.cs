using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Services.Planetside;
using Xunit;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models.GridMap;
using FluentAssertions;

namespace Voidwell.DaybreakGames.Test
{
    public class HexGridServiceTests
    {
        private readonly IMapRepository _mapRepository;

        private readonly HexGridService _sut;

        public HexGridServiceTests()
        {
            _mapRepository = Mock.Of<IMapRepository>();

            _sut = new HexGridService(_mapRepository);
        }

        [Fact]
        public async Task GetMapForZoneAsync_ReturnsZoneMapData()
        {
            // Arrange
            _mapRepository.AsMock()
                .Setup(a => a.GetMapHexsByZoneIdAsync(It.IsAny<int>()))
                .ReturnsAsync(RegionHexEntities);

            // Act
            var result = await _sut.GetMapForZoneAsync(2);

            // Assert
            result.First().Vertices.Should()
                .BeEquivalentTo(ExpectedRegionVertices, o => o.WithStrictOrdering());
        }

        private readonly List<MapHex> RegionHexEntities = new()
        {
            new(){ XPos = -2, YPos = 16 },
            new(){ XPos = -2, YPos = 17 },
            new(){ XPos = -1, YPos = 15 },
            new(){ XPos = -1, YPos = 16 },
            new(){ XPos = -1, YPos = 17 },
            new(){ XPos = 0, YPos = 14 },
            new(){ XPos = 0, YPos = 15 },
            new(){ XPos = 0, YPos = 16 },
            new(){ XPos = 1, YPos = 13 },
            new(){ XPos = 1, YPos = 14 },
            new(){ XPos = 1, YPos = 15 },
            new(){ XPos = 1, YPos = 16 },
        };

        private readonly List<GridMapVertex> ExpectedRegionVertices = new()
        {
            new() { X = 88.407, Y = 34.375 },
            new() { X = 86.603, Y = 37.5 },
            new() { X = 82.994, Y = 37.5 },
            new() { X = 81.19, Y = 40.625 },
            new() { X = 77.581, Y = 40.625 },
            new() { X = 75.777, Y = 43.75 },
            new() { X = 72.169, Y = 43.75 },
            new() { X = 70.365, Y = 46.875 },
            new() { X = 72.169, Y = 50 },
            new() { X = 75.777, Y = 50 },
            new() { X = 77.581, Y = 53.125 },
            new() { X = 81.19, Y = 53.125 },
            new() { X = 82.994, Y = 56.25 },
            new() { X = 86.603, Y = 56.25 },
            new() { X = 88.407, Y = 59.375 },
            new() { X = 92.015, Y = 59.375 },
            new() { X = 93.819, Y = 56.25 },
            new() { X = 92.015, Y = 53.125 },
            new() { X = 93.819, Y = 50 },
            new() { X = 97.428, Y = 50 },
            new() { X = 99.232, Y = 46.875 },
            new() { X = 97.428, Y = 43.75 },
            new() { X = 99.232, Y = 40.625 },
            new() { X = 97.428, Y = 37.5 },
            new() { X = 93.819, Y = 37.5 },
            new() { X = 92.015, Y = 34.375 }
        };
    }
}
