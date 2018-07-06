using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Data.Repositories.Models;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WeaponService : IWeaponService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly IWeaponAggregateService _weaponAggregateService;
        private readonly ISanctionedWeaponsRepository _sanctionedWeaponsRepository;
        private readonly IEventRepository _eventRepository;
        private readonly CensusItem _censusItem;
        private readonly ICache _cache;

        private readonly string _weaponInfoCacheKey = "ps2.weaponinfo";
        private readonly string _sanctionedWeaponsCacheKey = "ps2.sanctionedWeapons";
        private readonly TimeSpan _weaponInfoCacheExpiration = TimeSpan.FromHours(8);
        private readonly TimeSpan _weaponLeaderboardCacheExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _sanctionedWeaponsCacheExpiration = TimeSpan.FromHours(8);

        public WeaponService(ICharacterRepository characterRepository, IWeaponAggregateService weaponAggregateService,
            ISanctionedWeaponsRepository sanctionedWeaponRepository, IEventRepository eventRepository, CensusItem censusItem, ICache cache)
        {
            _characterRepository = characterRepository;
            _weaponAggregateService = weaponAggregateService;
            _sanctionedWeaponsRepository = sanctionedWeaponRepository;
            _eventRepository = eventRepository;
            _censusItem = censusItem;
            _cache = cache;
        }

        public async Task<WeaponInfoResult> GetWeaponInfo(int weaponItemId)
        {
            var cachedInfo = await _cache.GetAsync<WeaponInfoResult>($"{_weaponInfoCacheKey}_{weaponItemId}");
            if (cachedInfo != null)
            {
                return cachedInfo;
            }

            var info = await _censusItem.GetWeaponInfo(weaponItemId);

            var hipModes = info.FireMode.Where(m => m.Type == "primary");
            var aimModes = info.FireMode.Where(m => m.Type == "secondary");

            var weaponInfo = new WeaponInfoResult
            {
                Name = info.Name.English,
                ItemId = weaponItemId,
                Category = info.Category.Name.English,
                FactionId = info.FactionId,
                ImageId = info.ImageId,
                Description = info.Description.English,
                MaxStackSize = info.MaxStackSize,
                Range = info.Datasheet.Range.English,
                FireRateMs = info.Datasheet.FireRateMs,
                ClipSize = info.Datasheet.ClipSize,
                Capacity = info.Datasheet.Capacity,
                MuzzleVelocity = info.FireMode.First().Speed,
                MinDamage = info.FireMode.Min(m => m.DamageMin),
                MaxDamage = info.FireMode.Max(m => m.DamageMax),
                MinDamageRange = info.FireMode.Min(m => m.DamageMinRange),
                MaxDamageRange = info.FireMode.Min(m => m.DamageMaxRange),
                MinReloadSpeed = info.FireMode.Min(m => m.ReloadTimeMs + m.ReloadChamberTimeMs),
                MaxReloadSpeed = info.FireMode.Max(m => m.ReloadTimeMs),
                IronSightZoom = aimModes.First().DefaultZoom,
                FireModes = hipModes.Select(m => m.Description.English),
                IsVehicleWeapon = info.IsVehicleWeapon
            };

            if (hipModes != null)
            {
                weaponInfo.HipAcc = new AccuracyState
                {
                    Crouching = hipModes.First().States.FirstOrDefault(s => s.PlayerState == "Crouching")?.MinConeOfFire,
                    CrouchWalking = hipModes.First().States.FirstOrDefault(s => s.PlayerState == "CrouchWalking")?.MinConeOfFire,
                    Standing = hipModes.First().States.FirstOrDefault(s => s.PlayerState == "Standing")?.MinConeOfFire,
                    Running = hipModes.First().States.FirstOrDefault(s => s.PlayerState == "Running")?.MinConeOfFire,
                    Cof = hipModes.First().CofRecoil
                };
            }

            if (aimModes != null)
            {
                weaponInfo.AimAcc = new AccuracyState
                {
                    Crouching = aimModes.First().States.FirstOrDefault(s => s.PlayerState == "Crouching")?.MinConeOfFire,
                    CrouchWalking = aimModes.First().States.FirstOrDefault(s => s.PlayerState == "CrouchWalking")?.MinConeOfFire,
                    Standing = aimModes.First().States.FirstOrDefault(s => s.PlayerState == "Standing")?.MinConeOfFire,
                    Running = aimModes.First().States.FirstOrDefault(s => s.PlayerState == "Running")?.MinConeOfFire,
                    Cof = aimModes.First().CofRecoil
                };
            }

            await _cache.SetAsync($"{_weaponInfoCacheKey}_{weaponItemId}", weaponInfo, _weaponInfoCacheExpiration);

            return weaponInfo;
        }

        public async Task<IEnumerable<WeaponLeaderboardRow>> GetLeaderboard(int weaponItemId, string sortColumn = "Kills", SortDirection sortDirection = SortDirection.Descending, int rowStart = 0, int limit = 250)
        {
            var cacheKey = $"{_weaponInfoCacheKey}_leaderboard_{weaponItemId}";

            var weaponRows = await _cache.GetAsync<IEnumerable<WeaponLeaderboardRow>>(cacheKey);
            if (weaponRows != null)
            {
                return weaponRows;
            }

            var weaponStats = await _characterRepository.GetCharacterWeaponLeaderboardAsync(weaponItemId, sortColumn, sortDirection, rowStart, limit);
            var aggregate = await _weaponAggregateService.GetAggregateForItem(weaponItemId);

            weaponRows = weaponStats.Select(s => ConvertToResult(s, aggregate));

            await _cache.SetAsync(cacheKey, weaponRows, _weaponLeaderboardCacheExpiration);

            return weaponRows;
        }

        public async Task<IEnumerable<int>> GetAllSanctionedWeaponIds()
        {
            var weapons = await _cache.GetAsync<IEnumerable<int>>(_sanctionedWeaponsCacheKey);
            if (weapons != null)
            {
                return weapons;
            }

            var repoWeapons = await _sanctionedWeaponsRepository.GetAllSanctionedWeapons();
            weapons = repoWeapons.Select(a => a.Id);
            
            if (weapons != null)
            {
                await _cache.SetAsync(_sanctionedWeaponsCacheKey, weapons, _sanctionedWeaponsCacheExpiration);
            }

            return weapons;
        }

        public async Task<Dictionary<int, IEnumerable<DailyWeaponStats>>> GetOracleStatsFromWeaponByDateAsync(IEnumerable<int> weaponIds, DateTime start, DateTime end)
        {
            var statTasks = weaponIds.Select(id => GetOracleStats(id, start, end)).ToArray();
            await Task.WhenAll(statTasks);

            var oracleDict = new Dictionary<int, IEnumerable<DailyWeaponStats>>();
            for (var i = 0; i < weaponIds.Count(); i++)
            {
                oracleDict[weaponIds.ToArray()[i]] = statTasks.ToArray()[i].Result;
            }

            return oracleDict;
        }

        private readonly KeyedSemaphoreSlim _oracleStatLock = new KeyedSemaphoreSlim();

        private async Task<IEnumerable<DailyWeaponStats>> GetOracleStats(int weaponId, DateTime start, DateTime end)
        {
            var cacheKey = $"ps2.oracle_{weaponId}_{start.Year}-{start.Month}-{start.Day}_{end.Year}-{end.Month}-{end.Day}";

            using (await _oracleStatLock.WaitAsync(cacheKey))
            {

                var stats = await _cache.GetAsync<IEnumerable<DailyWeaponStats>>(cacheKey);
                if (stats != null)
                {
                    return stats;
                }

                stats = await _eventRepository.GetDailyWeaponAggregatesByWeaponIdAsync(weaponId, start, end);
                if (stats != null)
                {
                    await _cache.SetAsync(cacheKey, stats, TimeSpan.FromDays(1));
                }

                Console.WriteLine(stats.Count());

                return stats;
            }
        }

        private WeaponLeaderboardRow ConvertToResult(CharacterWeaponStat model, WeaponAggregate aggregate)
        {
            double? kdrDelta = null;
            double? accuDelta = null;
            double? hsrDelta = null;
            double? kphDelta = null;

            if (aggregate != null)
            {

                if (model.Deaths > 0 && aggregate.STDKdr > 0)
                {
                    kdrDelta = (((double)model.Kills / (double)model.Deaths) - aggregate.AVGKdr) / aggregate.STDKdr;
                }

                if (model.FireCount > 0 && aggregate.STDAccuracy > 0)
                {
                    accuDelta = (((double)model.HitCount / (double)model.FireCount) - aggregate.AVGAccuracy) / aggregate.STDAccuracy;
                }

                if (model.Kills > 0 && aggregate.STDHsr > 0)
                {
                    hsrDelta = (((double)model.Headshots / (double)model.Kills) - aggregate.AVGHsr) / aggregate.STDHsr;
                }

                if (model.PlayTime > 0 && aggregate.STDKph > 0)
                {
                    kphDelta = (((double)model.Kills / ((double)model.PlayTime / 3600)) - aggregate.AVGKph) / aggregate.STDKph;
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
