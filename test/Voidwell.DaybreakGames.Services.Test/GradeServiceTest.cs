using FluentAssertions;
using Voidwell.DaybreakGames.Services.Planetside;
using Xunit;

namespace Voidwell.DaybreakGames.Services.Test;

public class GradeServiceTest
{
    private readonly GradeService _sut = new();

    [Theory]
    [InlineData(5.0, "S")]
    [InlineData(4.01, "S")]
    [InlineData(4.0, "M**")]
    [InlineData(3.5, "M**")]
    [InlineData(3.25, "M*")]
    [InlineData(3.0, "M++")]
    [InlineData(2.6, "M")]
    [InlineData(1.25, "A")]
    [InlineData(1.0, "B**")]
    [InlineData(0.5, "B+")]
    [InlineData(0.45, "B+")]
    [InlineData(0.4, "B")]
    [InlineData(0.0, "C+")]
    [InlineData(0.125, "C++")]
    [InlineData(-0.125, "C+")]
    [InlineData(-0.2, "C")]
    [InlineData(-0.7, "D")]
    [InlineData(-1.65, "F")]
    [InlineData(-3.0, "T")]
    [InlineData(-3.5, "T*")]
    public void GetGradeByDelta_ReturnsHighestGradeAtOrBelowDelta(double delta, string expected)
    {
        _sut.GetGradeByDelta(delta).Should().Be(expected);
    }

    [Theory]
    [InlineData(-3.6)]
    [InlineData(-100)]
    public void GetGradeByDelta_BelowLowestGrade_ReturnsLowestGrade(double delta)
    {
        _sut.GetGradeByDelta(delta).Should().Be("T*");
    }

    [Fact]
    public void GetGradeByDelta_NullDelta_ReturnsLowestGrade()
    {
        _sut.GetGradeByDelta(null).Should().Be("T*");
    }

    [Fact]
    public void GetGradeByDelta_IsMonotonic()
    {
        var grades = Enumerable.Range(-40, 90)
            .Select(i => i / 10.0)
            .Select(delta => _sut.GetAllGrades().ToList().FindIndex(g => g.Grade == _sut.GetGradeByDelta(delta)))
            .ToList();

        // Higher delta never maps to a grade listed later (lower) in the table
        grades.Zip(grades.Skip(1), (previous, next) => next <= previous || previous == -1).Should().OnlyContain(ok => ok);
    }

    [Fact]
    public void GetAllGrades_AreOrderedFromBestToWorst()
    {
        var deltas = _sut.GetAllGrades().Select(a => a.Delta).ToList();

        deltas.Should().BeInDescendingOrder();
        deltas.Should().HaveCount(37);
    }
}
