using Voidwell.DaybreakGames.Domain.Models;
using Xunit;

namespace Voidwell.DaybreakGames.Domain.Test;

public class MapScoreTest
{
    [Fact]
    public void OwnershipScoreFactions_CalculatesPercentOfTotal()
    {
        var sut = new OwnershipScoreFactions(vsScore: 2, ncScore: 2, trScore: 0, nsScore: 0, neuturalScore: 1);

        Assert.Equal(2, sut.Vs.Value);
        Assert.Equal(0.4f, sut.Vs.Percent, 0.0001f);
        Assert.Equal(0.4f, sut.Nc.Percent, 0.0001f);
        Assert.Equal(0f, sut.Tr.Percent);
        Assert.Equal(0f, sut.Ns.Percent);
        Assert.Equal(1, sut.Neutural.Value);
        Assert.Equal(0.2f, sut.Neutural.Percent, 0.0001f);
    }

    [Fact]
    public void OwnershipScoreFactions_PercentagesSumToOne()
    {
        var sut = new OwnershipScoreFactions(7, 11, 13, 3, 5);

        var sum = sut.Vs.Percent + sut.Nc.Percent + sut.Tr.Percent + sut.Ns.Percent + sut.Neutural.Percent;

        Assert.Equal(1f, sum, 0.0001f);
    }

    [Fact]
    public void OwnershipScoreFactions_AllZero_ProducesNaNPercentages()
    {
        // Documents current behavior: an empty map divides by zero rather than reporting 0%
        var sut = new OwnershipScoreFactions(0, 0, 0, 0, 0);

        Assert.True(float.IsNaN(sut.Vs.Percent));
    }
}
