using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WeaponService : IWeaponService
    {
        private readonly ISanctionedWeaponsRepository _sanctionedWeaponsRepository;
        private readonly IWorldEventsService _worldEventsService;
        private readonly IItemService _itemService;
        private readonly CensusItem _censusItem;
        private readonly ICache _cache;

        private const string _weaponInfoCacheKey = "ps2.weaponinfo";
        private const string _sanctionedWeaponsCacheKey = "ps2.sanctionedWeapons";
        private readonly TimeSpan _weaponInfoCacheExpiration = TimeSpan.FromHours(8);
        private readonly TimeSpan _sanctionedWeaponsCacheExpiration = TimeSpan.FromHours(8);

        public WeaponService(ISanctionedWeaponsRepository sanctionedWeaponRepository, IWorldEventsService worldEventsService,
            IItemService itemService, CensusItem censusItem, ICache cache)
        {
            _sanctionedWeaponsRepository = sanctionedWeaponRepository;
            _worldEventsService = worldEventsService;
            _itemService = itemService;
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

            var hipModes = info.FireMode.Where(m => m.Type == "primary").ToList();
            var aimModes = info.FireMode.Where(m => m.Type == "secondary").ToList();

            var minDamage = info.FireMode.Min(m => m.DamageMin) ?? info.Datasheet.DamageMin;
            var maxDamage = info.FireMode.Max(m => m.DamageMax) ?? info.Datasheet.DamageMax;

            var minReloadSpeed = info.FireMode.Any(a => a.ReloadTimeMs.HasValue && a.ReloadChamberTimeMs.HasValue) ? info.FireMode.Min(m => m.ReloadTimeMs.Value + m.ReloadChamberTimeMs.Value) : info.Datasheet.ReloadMsMin;
            var maxReloadSpeed = info.FireMode.Max(m => m.ReloadTimeMs) ?? info.Datasheet.ReloadMsMax;

            var weaponInfo = new WeaponInfoResult
            {
                Name = info.Name?.English,
                ItemId = weaponItemId,
                Category = info.Category?.Name?.English,
                FactionId = info.FactionId,
                ImageId = info.ImageId,
                Description = info.Description?.English,
                MaxStackSize = info.MaxStackSize,
                Range = info.Datasheet?.Range?.English,
                FireRateMs = info.Datasheet?.FireRateMs,
                ClipSize = info.Datasheet?.ClipSize,
                Capacity = info.Datasheet?.Capacity,
                MuzzleVelocity = info.FireMode?.FirstOrDefault()?.Speed,
                MinDamage = minDamage,
                MaxDamage = maxDamage,
                MinDamageRange = info.FireMode?.Min(m => m.DamageMinRange).GetValueOrDefault(),
                MaxDamageRange = info.FireMode?.Min(m => m.DamageMaxRange).GetValueOrDefault(),
                IndirectMinDamage = info.FireMode?.Min(m => m.IndirectDamageMin),
                IndirectMaxDamage = info.FireMode?.Max(m => m.IndirectDamageMax),
                IndirectMinDamageRange = info.FireMode?.Min(m => m.IndirectDamageMinRange),
                IndirectMaxDamageRange = info.FireMode?.Max(m => m.IndirectDamageMaxRange),
                MinReloadSpeed = minReloadSpeed,
                MaxReloadSpeed = maxReloadSpeed,
                IronSightZoom = aimModes?.FirstOrDefault()?.DefaultZoom,
                FireModes = hipModes?.Select(m => m.Description?.English),
                IsVehicleWeapon = info.IsVehicleWeapon,
                DamageRadius = info.FireMode?.Max(m => m.DamageRadius),
                HipAcc = GetAccuracyStateFromFireMode(hipModes?.FirstOrDefault()),
                AimAcc = GetAccuracyStateFromFireMode(aimModes?.FirstOrDefault())
            };

            await _cache.SetAsync($"{_weaponInfoCacheKey}_{weaponItemId}", weaponInfo, _weaponInfoCacheExpiration);

            return weaponInfo;
        }

        public async Task<WeaponInfoResult> GetWeaponInfoByName(string weaponName)
        {
            if (!int.TryParse(weaponName, out int weaponId))
            {
                var items = await _itemService.LookupWeaponsByName(weaponName, 25);
                if (items == null || !items.Any())
                {
                    return null;
                }
                weaponId = items.First().Id;
            }

            return await GetWeaponInfo(weaponId);
        }

        private static AccuracyState GetAccuracyStateFromFireMode(CensusWeaponInfoModel.WeaponFireMode mode)
        {
            if (mode == null)
            {
                return null;
            }

            return new AccuracyState
            {
                Crouching = mode.States.FirstOrDefault(s => s.PlayerState == "Crouching")?.MinConeOfFire,
                CrouchWalking = mode.States.FirstOrDefault(s => s.PlayerState == "CrouchWalking")?.MinConeOfFire,
                Standing = mode.States.FirstOrDefault(s => s.PlayerState == "Standing")?.MinConeOfFire,
                Running = mode.States.FirstOrDefault(s => s.PlayerState == "Running")?.MinConeOfFire,
                Cof = mode.CofRecoil
            };
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
            
            if (weapons.Any())
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

                stats = await _worldEventsService.GetDailyWeaponAggregatesByWeaponIdAsync(weaponId, start, end);
                if (stats != null)
                {
                    await _cache.SetAsync(cacheKey, stats, TimeSpan.FromHours(1));
                }

                return stats;
            }
        }
    }
}
