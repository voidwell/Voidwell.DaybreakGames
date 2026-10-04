using FluentAssertions;
using Voidwell.DaybreakGames.Domain.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

public class MapScoreTest
{
    [Fact]
    public void OwnershipScoreFactions_CalculatesPercentOfTotal()
    {
        var sut = new OwnershipScoreFactions(vsScore: 2, ncScore: 2, trScore: 0, nsScore: 0, neuturalScore: 1);

        sut.Vs.Value.Should().Be(2);
        sut.Vs.Percent.Should().BeApproximately(0.4f, 0.0001f);
        sut.Nc.Percent.Should().BeApproximately(0.4f, 0.0001f);
        sut.Tr.Percent.Should().Be(0f);
        sut.Ns.Percent.Should().Be(0f);
        sut.Neutural.Value.Should().Be(1);
        sut.Neutural.Percent.Should().BeApproximately(0.2f, 0.0001f);
    }

    [Fact]
    public void OwnershipScoreFactions_PercentagesSumToOne()
    {
        var sut = new OwnershipScoreFactions(7, 11, 13, 3, 5);

        var sum = sut.Vs.Percent + sut.Nc.Percent + sut.Tr.Percent + sut.Ns.Percent + sut.Neutural.Percent;

        sum.Should().BeApproximately(1f, 0.0001f);
    }

    [Fact]
    public void OwnershipScoreFactions_AllZero_ProducesNaNPercentages()
    {
        // Documents current behavior: an empty map divides by zero rather than reporting 0%
        var sut = new OwnershipScoreFactions(0, 0, 0, 0, 0);

        float.IsNaN(sut.Vs.Percent).Should().BeTrue();
    }
}
