namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions;

public interface ICharacterSessionService
{
    Task<IEnumerable<Data.Models.Planetside.PlayerSession>> GetCharacterSessionsAsync(long characterId, CancellationToken cancellationToken, int limit = 25, int page = 0);
    Task<Domain.Models.PlayerSession?> GetCharacterSessionAsync(long characterId, int sessionId, CancellationToken cancellationToken);
    Task<Domain.Models.PlayerSession?> GetCharacterLiveSessionAsync(long characterId, CancellationToken cancellationToken);
    Task<IEnumerable<Domain.Models.PlayerSessionSummary>> GetLatestCharacterSessionsAsync(long characterId, int limit, CancellationToken cancellationToken);
}
