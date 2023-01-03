using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Voidwell.DaybreakGames.CensusServices.Patcher;
using Xunit;

namespace Voidwell.DaybreakGames.Test.CensusServicesTests
{
    public class PatchUtilTest
    {
        [Fact]
        public void PatchData_target_exists_source_has_patch()
        {
            var target = TestData.Generate(2);
            var source = TestData.Generate(4);

            var expected = new[] { new TestData(0), new TestData(1), new TestData(2), new TestData(3) };

            var result = PatchUtil.PatchData<TestData>(x => x.Id, target, source);

            result.Should()
                .BeEquivalentTo(expected);
        }

        [Fact]
        public void PatchData_target_null_source_has_patch()
        {
            var target = Enumerable.Empty<TestData>();
            var source = TestData.Generate(4);

            var expected = new[] { new TestData(0), new TestData(1), new TestData(2), new TestData(3) };

            var result = PatchUtil.PatchData<TestData>(x => x.Id, target, source);

            result.Should()
                .BeEquivalentTo(expected);
        }

        [Fact]
        public void PatchData_target_exists_source_null()
        {
            var target = TestData.Generate(4);
            var source = Enumerable.Empty<TestData>();

            var expected = new[] { new TestData(0), new TestData(1), new TestData(2), new TestData(3) };

            var result = PatchUtil.PatchData<TestData>(x => x.Id, target, source);

            result.Should()
                .BeEquivalentTo(expected);
        }

        private class TestData
        {
            public int Id { get; set; }
            public object Value { get; set; }

            public TestData(int id)
            {
                Id = id;
                Value = id;
            }

            public static IEnumerable<TestData> Generate(int count)
            {
                for(var i = 0; i < count; i++)
                {
                    yield return new TestData(i);
                }
            }
        }
    }
}
