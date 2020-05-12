using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface ICharacterRatingService
    {
        Task<CharacterRating> GetRatingAsync(string characterId);
        Task SaveCachedRatingAsync(string characterId);
        Task CalculateRatingAsync(string winnerCharacterId, string loserCharacterId);
        Task<IEnumerable<RatingCharacterModel>> GetRatingsLeaderboardAsync(int limit);
    }
}