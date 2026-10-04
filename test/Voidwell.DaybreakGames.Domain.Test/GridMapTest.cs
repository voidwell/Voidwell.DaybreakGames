using FluentAssertions;
using Voidwell.DaybreakGames.Domain.Models.GridMap;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

public class GridMapTest
{
    [Fact]
    public void Vertex_Constructor_SwapsAxesAndRoundsToThreeDecimals()
    {
        var vertex = new GridMapVertex(1.23456, 7.89012);

        vertex.X.Should().Be(7.89);
        vertex.Y.Should().Be(1.235);
    }

    [Fact]
    public void Vertex_Equality_IsByCoordinates()
    {
        var a = new GridMapVertex(1, 2);
        var b = new GridMapVertex(1, 2);
        var c = new GridMapVertex(2, 1);

        a.Equals(b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
        a.Equals(c).Should().BeFalse();
        a.Equals(null).Should().BeFalse();
        a.Equals("not a vertex").Should().BeFalse();
    }

    [Fact]
    public void Vertex_NearlyEqualCoordinates_RoundToSameVertex()
    {
        new GridMapVertex(1.0001, 2.0002).Should().Be(new GridMapVertex(1.0004, 2.0001));
    }

    [Fact]
    public void Line_Equality_IgnoresDirection()
    {
        var a = new GridMapVertex(1, 1);
        var b = new GridMapVertex(2, 2);

        new GridMapLine(a, b).Equals(new GridMapLine(b, a)).Should().BeTrue();
        new GridMapLine(a, b).Equals(new GridMapLine(a, b)).Should().BeTrue();
    }

    [Fact]
    public void Line_Equality_DifferentVertices_IsFalse()
    {
        var a = new GridMapVertex(1, 1);
        var b = new GridMapVertex(2, 2);
        var c = new GridMapVertex(3, 3);

        new GridMapLine(a, b).Equals(new GridMapLine(a, c)).Should().BeFalse();
        new GridMapLine(a, b).Equals("not a line").Should().BeFalse();
    }

    [Fact]
    public void Line_ContainsVertex_ChecksBothEnds()
    {
        var a = new GridMapVertex(1, 1);
        var b = new GridMapVertex(2, 2);
        var line = new GridMapLine(a, b);

        line.ContainsVertex(a).Should().BeTrue();
        line.ContainsVertex(b).Should().BeTrue();
        line.ContainsVertex(new GridMapVertex(3, 3)).Should().BeFalse();
    }
}
