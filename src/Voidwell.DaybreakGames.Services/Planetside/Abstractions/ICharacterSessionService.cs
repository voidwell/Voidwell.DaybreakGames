namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions;

public interface ICharacterSessionService
{
    Task<IEnumerable<Data.Models.Planetside.PlayerSession>> GetCharacterSessionsAsync(string characterId, CancellationToken cancellationToken, int limit = 25, int page = 0);
    Task<Domain.Models.PlayerSession?> GetCharacterSessionAsync(string characterId, int sessionId, CancellationToken cancellationToken);
    Task<Domain.Models.PlayerSession?> GetCharacterLiveSessionAsync(string characterId, CancellationToken cancellationToken);
    Task<IEnumerable<Domain.Models.PlayerSessionSummary>> GetLatestCharacterSessionsAsync(string characterId, int limit, CancellationToken cancellationToken);
}
