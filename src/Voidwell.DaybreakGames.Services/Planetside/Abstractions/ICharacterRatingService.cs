using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside.Abstractions;

public interface ICharacterRatingService
{
    Task<CharacterRating> GetRatingAsync(long characterId);
    Task SaveCachedRatingAsync(long characterId);
    Task CalculateRatingAsync(long winnerCharacterId, long loserCharacterId);
    Task<IEnumerable<RatingCharacterModel>> GetRatingsLeaderboardAsync(int limit);
}
