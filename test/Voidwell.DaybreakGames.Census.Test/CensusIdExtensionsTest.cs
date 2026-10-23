using Voidwell.DaybreakGames.Census.Models.Extensions;
using Xunit;

namespace Voidwell.DaybreakGames.Census.Test;

public class CensusIdExtensionsTest
{
    [Theory]
    [InlineData("5428010618035323201", 5428010618035323201)]
    [InlineData("37509488620604883", 37509488620604883)]
    [InlineData("0", 0)]
    public void ToCensusId_ParsesDigits(string value, long expected)
    {
        Assert.Equal(expected, value.ToCensusId());
        Assert.Equal(expected, value.ToOptionalCensusId());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ToOptionalCensusId_MissingValue_IsNull(string value)
    {
        Assert.Null(value.ToOptionalCensusId());
        Assert.Throws<FormatException>(() => value.ToCensusId());
    }

    [Theory]
    [InlineData("abc")]
    [InlineData(" 1")]
    [InlineData("-1")]
    [InlineData("9223372036854775808")]
    public void InvalidValues_Throw(string value)
    {
        Assert.Throws<FormatException>(() => value.ToOptionalCensusId());
        Assert.Throws<FormatException>(() => value.ToCensusId());
    }
}
