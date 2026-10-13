using Xunit;

namespace Voidwell.DaybreakGames.Utils.Test;

public class EnumerableExtensionsTest
{
    [Fact]
    public void SetGroupJoin_MapsJoinedData()
    {
        // Arrange
        var parents = GetTestParents();
        var children = GetTestChildren();
        var expected = GetExpectedResult();

        // Act
        parents.SetGroupJoin(children, a => a.Id, a => a.ParentId, a => a.Children);

        //Assert
        Assert.Equivalent(expected, parents);
    }

    [Fact]
    public void SetGroupJoin_NullInner_LeavesOuterUntouched()
    {
        var parents = GetTestParents();

        parents.SetGroupJoin(null, a => a.Id, a => a.ParentId, a => a.Children);

        Assert.All(parents, a => Assert.Empty(a.Children));
    }

    [Fact]
    public void SetGroupJoin_ParentWithoutChildren_GetsEmptyList()
    {
        var parents = new[] { new SimpleParent(99) };

        parents.SetGroupJoin(GetTestChildren(), a => a.Id, a => a.ParentId, a => a.Children);

        Assert.Empty(parents[0].Children);
    }

    [Fact]
    public void SelectManyNotNull_SkipsNullCollections()
    {
        var source = new[] { new[] { 1, 2 }, null, new[] { 3 } };

        var result = source.SelectManyNotNull(a => a);

        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void SelectManyNotNull_EmptySource_ReturnsEmpty()
    {
        Assert.Empty(Array.Empty<int[]>().SelectManyNotNull(a => a));
    }

    private static IEnumerable<SimpleParent> GetTestParents()
    {
        return new[]
        {
            new SimpleParent(1),
            new SimpleParent(2),
            new SimpleParent(3)
        };
    }

    private static IEnumerable<SimpleChild> GetTestChildren()
    {
        return new[]
        {
            new SimpleChild(1, 1),
            new SimpleChild(1, 2),
            new SimpleChild(1, 3),
            new SimpleChild(2, 1),
            new SimpleChild(4, 1),
            new SimpleChild(4, 2)
        };
    }

    private static IEnumerable<SimpleParent> GetExpectedResult()
    {
        return new[]
        {
            new SimpleParent(1)
            {
                Children = new[]
                {
                    new SimpleChild(1, 1),
                    new SimpleChild(1, 2),
                    new SimpleChild(1, 3)
                }
            },
            new SimpleParent(2)
            {
                Children = new[]
                {
                    new SimpleChild(2, 1)
                }
            },
            new SimpleParent(3)
        };
    }

    private class SimpleParent
    {
        public SimpleParent(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
        public IEnumerable<SimpleChild> Children { get; set; } = new List<SimpleChild>();
    }

    private class SimpleChild
    {
        public SimpleChild(int parentId, int id)
        {
            ParentId = parentId;
            Id = id;
        }

        public int ParentId { get; set; }
        public int Id { get; set; }
    }
}
