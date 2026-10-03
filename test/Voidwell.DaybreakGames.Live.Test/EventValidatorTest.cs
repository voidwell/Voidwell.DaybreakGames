using FluentAssertions;
using Voidwell.DaybreakGames.Live.CensusStream;
using Xunit;

namespace Voidwell.DaybreakGames.Live.Test;

public class EventValidatorTest
{
    private sealed record LoginEvent(string CharacterId, DateTime Timestamp);
    private sealed record LogoutEvent(string CharacterId, DateTime Timestamp);

    private static readonly Func<LoginEvent, bool> NeverExpire = _ => false;

    [Fact]
    public async Task Validiate_FirstEvent_IsValid()
    {
        using var sut = new EventValidator();

        (await sut.Validiate(new LoginEvent("c1", DateTime.UtcNow), a => a.CharacterId, NeverExpire)).Should().BeTrue();
    }

    [Fact]
    public async Task Validiate_DuplicateEvent_IsInvalid()
    {
        using var sut = new EventValidator();
        var ev = new LoginEvent("c1", DateTime.UtcNow);

        await sut.Validiate(ev, a => a.CharacterId, NeverExpire);

        (await sut.Validiate(new LoginEvent("c1", DateTime.UtcNow), a => a.CharacterId, NeverExpire)).Should().BeFalse();
    }

    [Fact]
    public async Task Validiate_DifferentKeys_AreIndependent()
    {
        using var sut = new EventValidator();

        await sut.Validiate(new LoginEvent("c1", DateTime.UtcNow), a => a.CharacterId, NeverExpire);

        (await sut.Validiate(new LoginEvent("c2", DateTime.UtcNow), a => a.CharacterId, NeverExpire)).Should().BeTrue();
    }

    [Fact]
    public async Task Validiate_SameKeyDifferentEventType_IsIndependent()
    {
        using var sut = new EventValidator();

        await sut.Validiate(new LoginEvent("c1", DateTime.UtcNow), a => a.CharacterId, NeverExpire);

        (await sut.Validiate(new LogoutEvent("c1", DateTime.UtcNow), a => a.CharacterId, _ => false)).Should().BeTrue();
    }

    [Fact]
    public async Task Validiate_DuplicateOfExpiredEvent_IsInvalidButReplacesTheExpiredEntry()
    {
        using var sut = new EventValidator();
        var old = new LoginEvent("c1", DateTime.UtcNow.AddSeconds(-10));
        await sut.Validiate(old, a => a.CharacterId, a => DateTime.UtcNow - a.Timestamp > TimeSpan.FromSeconds(1));

        // The second event is a duplicate, but it is also the moment the first one is cleaned up as expired
        var duplicate = await sut.Validiate(new LoginEvent("c1", DateTime.UtcNow), a => a.CharacterId, a => DateTime.UtcNow - a.Timestamp > TimeSpan.FromSeconds(1));
        var afterCleanup = await sut.Validiate(new LoginEvent("c1", DateTime.UtcNow), a => a.CharacterId, a => DateTime.UtcNow - a.Timestamp > TimeSpan.FromSeconds(1));

        duplicate.Should().BeFalse();
        afterCleanup.Should().BeFalse();
    }

    [Fact]
    public async Task Validiate_ConcurrentDuplicates_OnlyOneIsValid()
    {
        using var sut = new EventValidator();

        var results = await Task.WhenAll(Enumerable.Range(0, 20)
            .Select(_ => sut.Validiate(new LoginEvent("c1", DateTime.UtcNow), a => a.CharacterId, NeverExpire)));

        results.Count(valid => valid).Should().Be(1);
    }
}
