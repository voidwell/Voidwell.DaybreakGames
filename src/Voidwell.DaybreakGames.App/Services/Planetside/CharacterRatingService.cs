using Glicko2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CharacterRatingService : ICharacterRatingService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly ICache _cache;

        private const double DefaultRating = 1500;
        private const double DefaultDeviation = 100;
        private const double DefaultVolatility = 0.02;

        private static Func<string, string> GetCacheKey => characterId => $"ps2.characterRating_{characterId}";
        private const string _leaderboardCacheKey = "ps2.characterRatingLeaderboard";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(1);
        private readonly TimeSpan _leaderboardCacheExpiration = TimeSpan.FromMinutes(5);

        private readonly KeyedSemaphoreSlim _calculatingLock = new KeyedSemaphoreSlim();

        public CharacterRatingService(ICharacterRepository characterRepository, ICache cache)
        {
            _characterRepository = characterRepository;
            _cache = cache;
        }

        public async Task CalculateRatingAsync(string winnerCharacterId, string loserCharacterId)
        {
            var locks = await Task.WhenAll(_calculatingLock.WaitAsync(winnerCharacterId), _calculatingLock.WaitAsync(loserCharacterId));

            try
            {
                var winnerRatingTask = GetRatingAsync(winnerCharacterId);
                var loserRatingTask = GetRatingAsync(loserCharacterId);

                await Task.WhenAll(winnerRatingTask, loserRatingTask);

                var winnerRating = winnerRatingTask.Result;
                var loserRating = loserRatingTask.Result;

                var winnerResult = Calculate1v1(winnerRating, loserRating, true);
                var loserResult = Calculate1v1(loserRating, winnerRating, false);

                await Task.WhenAll(_cache.SetAsync(GetCacheKey(winnerCharacterId), winnerResult, _cacheExpiration),
                    _cache.SetAsync(GetCacheKey(loserCharacterId), loserResult, _cacheExpiration));
            }
            finally
            {
                locks.ToList().ForEach(a => a.Dispose());
            }
        }

        public async Task<CharacterRating> GetRatingAsync(string characterId)
        {
            var cacheKey = GetCacheKey(characterId);

            var rating = await _cache.GetAsync<CharacterRating>(cacheKey);
            if (rating == null)
            {
                rating = await _characterRepository.GetCharacterRatingAsync(characterId) ?? new CharacterRating { CharacterId = characterId, Rating = DefaultRating, Deviation = DefaultDeviation, Volatility = DefaultVolatility };

                await _cache.SetAsync(cacheKey, rating, _cacheExpiration);
            }

            return rating;
        }

        public async Task SaveCachedRatingAsync(string characterId)
        {
            using (await _calculatingLock.WaitAsync(characterId))
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
            var cacheLeaderboard = await _cache.GetAsync<IEnumerable<RatingCharacterModel>>(_leaderboardCacheKey);
            if (cacheLeaderboard != null)
            {
                return cacheLeaderboard;
            }

            var results = await _characterRepository.GetCharacterRatingLeaderboardAsync(limit);

            var leaderboard = results.Select(a => new RatingCharacterModel
            {
                CharacterId = a.CharacterId,
                Rating = a.Rating,
                Deviation = a.Deviation,
                Name = a.Character?.Name,
                BattleRank = a.Character?.BattleRank,
                WorldId = a.Character?.WorldId,
                FactionId = a.Character?.FactionId
            }).ToList();

            if (leaderboard.Any())
            {
                await _cache.SetAsync(_leaderboardCacheKey, leaderboard, _leaderboardCacheExpiration);
            }

            return leaderboard;
        }

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
    }
}
