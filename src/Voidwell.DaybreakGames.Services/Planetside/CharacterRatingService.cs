//using Glicko2;
using AsyncKeyedLock;
using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside;

public class CharacterRatingService : ICharacterRatingService
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ICache _cache;

    private const double _defaultRating = 1500;
    private const double _defaultDeviation = 100;
    private const double _defaultVolatility = 0.02;

    private static Func<string, string> GetCacheKey => characterId => $"ps2.characterRating_{characterId}";
    private const string _leaderboardCacheKey = "ps2.characterRatingLeaderboard";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(1);
    private readonly TimeSpan _leaderboardCacheExpiration = TimeSpan.FromMinutes(5);

    private readonly AsyncKeyedLocker<string> _calculatingLock = new();

    public CharacterRatingService(ICharacterRepository characterRepository, ICache cache)
    {
        _characterRepository = characterRepository;
        _cache = cache;
    }

    public async Task CalculateRatingAsync(string winnerCharacterId, string loserCharacterId)
    {
        var locks = await Task.WhenAll(_calculatingLock.LockAsync(winnerCharacterId).AsTask(), _calculatingLock.LockAsync(loserCharacterId).AsTask());

        try
        {
            var winnerRatingTask = GetRatingAsync(winnerCharacterId);
            var loserRatingTask = GetRatingAsync(loserCharacterId);

            await Task.WhenAll(winnerRatingTask, loserRatingTask);

            var winnerRating = winnerRatingTask.Result;
            var loserRating = loserRatingTask.Result;

            //var winnerResult = Calculate1v1(winnerRating, loserRating, true);
            //var loserResult = Calculate1v1(loserRating, winnerRating, false);

            //await Task.WhenAll(_cache.SetAsync(GetCacheKey(winnerCharacterId), winnerResult, _cacheExpiration),
            //    _cache.SetAsync(GetCacheKey(loserCharacterId), loserResult, _cacheExpiration));
        }
        finally
        {
            locks.ToList().ForEach(a => a.Dispose());
        }
    }

    public async Task<CharacterRating> GetRatingAsync(string characterId)
    {
        var cacheKey = GetCacheKey(characterId);

        return await _cache.GetOrSetAsync(
            cacheKey,
            async ct => await _characterRepository.GetCharacterRatingAsync(characterId) ?? new CharacterRating { CharacterId = characterId, Rating = _defaultRating, Deviation = _defaultDeviation, Volatility = _defaultVolatility },
            _cacheExpiration);
    }

    public async Task SaveCachedRatingAsync(string characterId)
    {
        using (await _calculatingLock.LockAsync(characterId))
        {
            var cacheKey = GetCacheKey(characterId);

            var rating = await _cache.GetAsync<CharacterRating>(cacheKey);
            if (rating == null)
            {
                return;
            }

            await _characterRepository.UpsertAsync(rating);
            await _cache.RemoveAsync(cacheKey);
        }
    }

    public async Task<IEnumerable<RatingCharacterModel>> GetRatingsLeaderboardAsync(int limit)
    {
        return await _cache.GetOrSetAsync<IEnumerable<RatingCharacterModel>>(
            _leaderboardCacheKey,
            async ct =>
            {
                var results = await _characterRepository.GetCharacterRatingLeaderboardAsync(limit);

                return results.Select(a => new RatingCharacterModel
                {
                    CharacterId = a.CharacterId,
                    Rating = a.Rating,
                    Deviation = a.Deviation,
                    Name = a.Character?.Name,
                    BattleRank = a.Character?.BattleRank,
                    WorldId = a.Character?.WorldId,
                    FactionId = a.Character?.FactionId
                }).ToList();
            },
            _leaderboardCacheExpiration,
            leaderboard => leaderboard.Any());
    }

    /*
    private static CharacterRating Calculate1v1(CharacterRating focus, CharacterRating opponent, bool isWin)
    {
        var focusPlayer = ToGlickoPlayer(focus);
        var opponentPlayer = ToGlickoPlayer(opponent);

        var opponents = new List<GlickoOpponent> { new GlickoOpponent(opponentPlayer, isWin ? 1 : 0) };
        var result = GlickoCalculator.CalculateRanking(focusPlayer, opponents);

        return new CharacterRating
        {
            CharacterId = result.Name,
            Rating = result.Rating,
            Deviation = result.RatingDeviation,
            Volatility = result.Volatility
        };
    }

    private static GlickoPlayer ToGlickoPlayer(CharacterRating rating)
    {
        return new GlickoPlayer
        {
            Name = rating.CharacterId,
            Rating = rating.Rating,
            RatingDeviation = rating.Deviation,
            Volatility = rating.Volatility
        };
    }
    */
}
