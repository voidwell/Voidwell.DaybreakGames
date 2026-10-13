using Voidwell.DaybreakGames.Domain.Models.GridMap;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

public class GridMapTest
{
    [Fact]
    public void Vertex_Constructor_SwapsAxesAndRoundsToThreeDecimals()
    {
        var vertex = new GridMapVertex(1.23456, 7.89012);

        Assert.Equal(7.89, vertex.X);
        Assert.Equal(1.235, vertex.Y);
    }

    [Fact]
    public void Vertex_Equality_IsByCoordinates()
    {
        var a = new GridMapVertex(1, 2);
        var b = new GridMapVertex(1, 2);
        var c = new GridMapVertex(2, 1);

        Assert.True(a.Equals(b));
        Assert.Equal(b.GetHashCode(), a.GetHashCode());
        Assert.False(a.Equals(c));
        Assert.False(a.Equals(null));
        Assert.False(a.Equals("not a vertex"));
    }

    [Fact]
    public void Vertex_NearlyEqualCoordinates_RoundToSameVertex()
    {
        Assert.Equal(new GridMapVertex(1.0004, 2.0001), new GridMapVertex(1.0001, 2.0002));
    }

    [Fact]
    public void Line_Equality_IgnoresDirection()
    {
        var a = new GridMapVertex(1, 1);
        var b = new GridMapVertex(2, 2);

        Assert.True(new GridMapLine(a, b).Equals(new GridMapLine(b, a)));
        Assert.True(new GridMapLine(a, b).Equals(new GridMapLine(a, b)));
    }

    [Fact]
    public void Line_Equality_DifferentVertices_IsFalse()
    {
        var a = new GridMapVertex(1, 1);
        var b = new GridMapVertex(2, 2);
        var c = new GridMapVertex(3, 3);

        Assert.False(new GridMapLine(a, b).Equals(new GridMapLine(a, c)));
        Assert.False(new GridMapLine(a, b).Equals("not a line"));
    }

    [Fact]
    public void Line_ContainsVertex_ChecksBothEnds()
    {
        var a = new GridMapVertex(1, 1);
        var b = new GridMapVertex(2, 2);
        var line = new GridMapLine(a, b);

        Assert.True(line.ContainsVertex(a));
        Assert.True(line.ContainsVertex(b));
        Assert.False(line.ContainsVertex(new GridMapVertex(3, 3)));
    }
}
