using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.CensusStore.Services;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ICharacterStore _characterStore;
        private readonly IWeaponAggregateService _weaponAggregateService;
        private readonly ICache _cache;

        private const string _cacheKey = "ps2.leaderboard";
        private readonly Func<int, int, string, string, string> _getCharacterLeaderboardCacheKey = (weaponId, page, sort, sortDir) => $"{_cacheKey}_leaderboard_{weaponId}_{page}_{sort}_{sortDir}";
        private readonly TimeSpan _cacheCharacterLeaderboardExpiration = TimeSpan.FromMinutes(1);

        public LeaderboardService(ICharacterStore characterStore, IWeaponAggregateService weaponAggregateService, ICache cache)
        {
            _characterStore = characterStore;
            _weaponAggregateService = weaponAggregateService;
            _cache = cache;
        }

        public async Task<IEnumerable<WeaponLeaderboardRow>> GetCharacterWeaponLeaderboardAsync(int weaponItemId, int page = 0, int limit = 50, string sort = "kills", string sortDir = "desc")
        {
            var cacheKey = _getCharacterLeaderboardCacheKey(weaponItemId, page, sort, sortDir);

            var result = await _cache.GetAsync<IEnumerable<WeaponLeaderboardRow>>(cacheKey);
            if (result != null)
            {
                return result;
            }

            var stats = await _characterStore.GetCharacterWeaponLeaderboardAsync(weaponItemId, page, limit, sort, sortDir);
            if (stats == null || !stats.Any())
            {
                return Enumerable.Empty<WeaponLeaderboardRow>();
            }

            var aggregate = await _weaponAggregateService.GetAggregateForItem(weaponItemId);
            result = stats.Select(s => ConvertToLeaderboardRow(s, aggregate));

            await _cache.SetAsync(cacheKey, result, _cacheCharacterLeaderboardExpiration);

            return result;
        }

        private static WeaponLeaderboardRow ConvertToLeaderboardRow(CharacterWeaponStat model, WeaponAggregate aggregate)
        {
            double? kdrDelta = null;
            double? accuDelta = null;
            double? hsrDelta = null;
            double? kphDelta = null;

            if (aggregate != null)
            {

                if (model.Deaths > 0 && aggregate.STDKdr > 0)
                {
                    kdrDelta = ((double)model.Kills / (double)model.Deaths - aggregate.AVGKdr) / aggregate.STDKdr;
                }

                if (model.FireCount > 0 && aggregate.STDAccuracy > 0)
                {
                    accuDelta = ((double)model.HitCount / (double)model.FireCount - aggregate.AVGAccuracy) / aggregate.STDAccuracy;
                }

                if (model.Kills > 0 && aggregate.STDHsr > 0)
                {
                    hsrDelta = ((double)model.Headshots / (double)model.Kills - aggregate.AVGHsr) / aggregate.STDHsr;
                }

                if (model.PlayTime > 0 && aggregate.STDKph > 0)
                {
                    kphDelta = ((double)model.Kills / ((double)model.PlayTime / 3600) - aggregate.AVGKph) / aggregate.STDKph;
                }
            }

            return new WeaponLeaderboardRow
            {
                CharacterId = model.CharacterId,
                Name = model.Character.Name,
                FactionId = model.Character.FactionId,
                WorldId = model.Character.WorldId,
                Kills = model.Kills.GetValueOrDefault(),
                Deaths = model.Deaths.GetValueOrDefault(),
                Headshots = model.Headshots.GetValueOrDefault(),
                ShotsFired = model.FireCount.GetValueOrDefault(),
                ShotsHit = model.HitCount.GetValueOrDefault(),
                PlayTime = model.PlayTime.GetValueOrDefault(),
                Score = model.Score.GetValueOrDefault(),
                VehicleKills = model.VehicleKills.GetValueOrDefault(),
                KdrDelta = kdrDelta,
                AccuracyDelta = accuDelta,
                HsrDelta = hsrDelta,
                KphDelta = kphDelta
            };
        }
    }
}
