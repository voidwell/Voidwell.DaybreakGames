using DaybreakGames.Census.Exceptions;
using Moq;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Xunit;

namespace Voidwell.DaybreakGames.CensusStore.Test;

public class CharacterDirectiveStoreTest
{
    private const long _characterId = 5428779225270452929;

    private readonly Mock<ICharacterDirectiveRepository> _repository = new();
    private readonly Mock<ICharacterStore> _characterStore = new();
    private readonly Mock<IStoreUpdaterHelper> _storeUpdaterHelper = new();

    private CharacterDirectiveStore CreateSut(Exception censusFailure)
    {
        // The Census collections are only used inside the lambdas passed to the updater helper, which is mocked here.
        _storeUpdaterHelper.Setup(a => a.UpdateAsync<CensusCharacterDirectiveModel, CharacterDirective>(It.IsAny<Func<Task<IEnumerable<CensusCharacterDirectiveModel>>>>())).ThrowsAsync(censusFailure);
        _storeUpdaterHelper.Setup(a => a.UpdateAsync<CensusCharacterDirectiveTierModel, CharacterDirectiveTier>(It.IsAny<Func<Task<IEnumerable<CensusCharacterDirectiveTierModel>>>>())).Returns(Task.CompletedTask);
        _storeUpdaterHelper.Setup(a => a.UpdateAsync<CensusCharacterDirectiveObjectiveModel, CharacterDirectiveObjective>(It.IsAny<Func<Task<IEnumerable<CensusCharacterDirectiveObjectiveModel>>>>())).Returns(Task.CompletedTask);
        _storeUpdaterHelper.Setup(a => a.UpdateAsync<CensusCharacterDirectiveTreeModel, CharacterDirectiveTree>(It.IsAny<Func<Task<IEnumerable<CensusCharacterDirectiveTreeModel>>>>())).Returns(Task.CompletedTask);

        return new CharacterDirectiveStore(_repository.Object, null!, null!, null!, null!, _characterStore.Object, _storeUpdaterHelper.Object);
    }

    [Fact]
    public async Task GetCharacterDirectives_WhenCensusTimesOut_ReturnsNull()
    {
        var timeout = new TaskCanceledException("The request was canceled due to the configured HttpClient.Timeout of 10 seconds elapsing.", new TimeoutException());

        var result = await CreateSut(timeout).GetCharacterDirectivesAsync(_characterId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCharacterDirectives_WhenCensusIsUnreachable_ReturnsNull()
    {
        var result = await CreateSut(new CensusConnectionException("Census unavailable")).GetCharacterDirectivesAsync(_characterId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetCharacterDirectives_WhenTheCallerCancels_StillThrows()
    {
        // A cancellation that is not an HttpClient timeout is not a Census failure and must not be swallowed.
        var cancellation = new TaskCanceledException("canceled");

        await Assert.ThrowsAsync<TaskCanceledException>(() => CreateSut(cancellation).GetCharacterDirectivesAsync(_characterId));
    }
}
