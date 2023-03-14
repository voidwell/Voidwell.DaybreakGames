using FluentAssertions;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Utils;
using Xunit;

namespace Voidwell.DaybreakGames.Test.Utils
{
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
            parents.Should()
                .BeEquivalentTo(expected);
        }

        private IEnumerable<SimpleParent> GetTestParents()
        {
            return new[]
            {
                new SimpleParent(1),
                new SimpleParent(2),
                new SimpleParent(3)
            };
        }

        private IEnumerable<SimpleChild> GetTestChildren()
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

        private IEnumerable<SimpleParent> GetExpectedResult()
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
}
