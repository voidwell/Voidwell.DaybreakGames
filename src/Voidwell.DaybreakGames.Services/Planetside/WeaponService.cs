using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Microservice.Cache;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.Microservice.Utility;
using static Voidwell.DaybreakGames.Census.Models.Extensions.CensusWeaponInfoModelExtensions;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WeaponService : IWeaponService
    {
        private readonly ISanctionedWeaponsRepository _sanctionedWeaponsRepository;
        private readonly IWorldEventsService _worldEventsService;
        private readonly IItemService _itemService;
        private readonly ICache _cache;
        private readonly ILogger _logger;

        private const string _weaponInfoCacheKey = "ps2.weaponinfo";
        private const string _sanctionedWeaponsCacheKey = "ps2.sanctionedWeapons";
        private const string _masterSanctionedWeaponsEndpoint = "https://raw.githubusercontent.com/cooltrain7/Planetside-2-API-Tracker/master/Weapons/sanction-list-machine.json";
        private readonly TimeSpan _weaponInfoCacheExpiration = TimeSpan.FromHours(8);
        private readonly TimeSpan _sanctionedWeaponsCacheExpiration = TimeSpan.FromHours(8);

        private readonly KeyedSemaphoreSlim _oracleStatLock = new KeyedSemaphoreSlim();
        private readonly SemaphoreSlim _sanctionedStoreLock = new SemaphoreSlim(1);

        public WeaponService(ISanctionedWeaponsRepository sanctionedWeaponRepository, IWorldEventsService worldEventsService,
            IItemService itemService, ICache cache, ILogger<WeaponService> logger)
        {
            _sanctionedWeaponsRepository = sanctionedWeaponRepository;
            _worldEventsService = worldEventsService;
            _itemService = itemService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<WeaponInfoResult> GetWeaponInfo(int weaponItemId)
        {
            var cachedInfo = await _cache.GetAsync<WeaponInfoResult>($"{_weaponInfoCacheKey}_{weaponItemId}");
            if (cachedInfo != null)
            {
                return cachedInfo;
            }

            var info = await _itemService.GetWeaponInfoAsync(weaponItemId);
            if (info == null)
            {
                return null;
            }

            var hipModes = info.GetFireModesOfType(FireModeType.Primary)?.ToList();
            var aimModes = info.GetFireModesOfType(FireModeType.Secondary)?.ToList();

            var weaponInfo = new WeaponInfoResult
            {
                Name = info.GetName(),
                ItemId = weaponItemId,
                Category = info.GetCategory(),
                FactionId = info.FactionId,
                ImageId = info.ImageId,
                Description = info.GetDescription(),
                MaxStackSize = info.MaxStackSize,
                Range = info.GetRange(),
                FireRateMs = info.Datasheet?.FireRateMs,
                ClipSize = info.Datasheet?.ClipSize,
                Capacity = info.Datasheet?.Capacity,
                MuzzleVelocity = info.GetWeaponSpeed(),
                MinDamage = info.GetMinDamage(),
                MaxDamage = info.GetMaxDamage(),
                MinDamageRange = info.GetMinDamageRange(),
                MaxDamageRange = info.GetMaxDamageRange(),
                IndirectMinDamage = info.GetIndirectMinDamage(),
                IndirectMaxDamage = info.GetIndirectMaxDamage(),
                IndirectMinDamageRange = info.GetIndirectMinDamageRange(),
                IndirectMaxDamageRange = info.GetIndirectMaxDamageRange(),
                MinReloadSpeed = info.GetMinReloadSpeed(),
                MaxReloadSpeed = info.GetMaxReloadSpeed(),
                IronSightZoom = aimModes?.GetDefaultZoom(),
                FireModes = hipModes?.GetFireModeNames(),
                IsVehicleWeapon = info.IsVehicleWeapon,
                DamageRadius = info.GetDamageRadius(),
                HipAcc = GetAccuracyStateFromFireMode(hipModes),
                AimAcc = GetAccuracyStateFromFireMode(aimModes)
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

        private static AccuracyState GetAccuracyStateFromFireMode(IEnumerable<CensusWeaponInfoModel.WeaponFireMode> modes)
        {
            if (modes?.Any() != true)
            {
                return null;
            }

            var mode = modes.FirstOrDefault();

            return new AccuracyState
            {
                Crouching = mode.GetFireModeStateCoF(FireModeState.Crouching),
                CrouchWalking = mode.GetFireModeStateCoF(FireModeState.CrouchWalking),
                Standing = mode.GetFireModeStateCoF(FireModeState.Standing),
                Running = mode.GetFireModeStateCoF(FireModeState.Running),
                Cof = mode.CofRecoil
            };
        }

        public async Task<IEnumerable<int>> GetAllSanctionedWeaponIds()
        {
            await _sanctionedStoreLock.WaitAsync();

            try
            {
                var weapons = await _cache.GetAsync<IEnumerable<int>>(_sanctionedWeaponsCacheKey);
                if (weapons != null)
                {
                    return weapons;
                }

                weapons = await GetSanctionedWeaponsFromMasterListAsync();
                if (weapons == null)
                {
                    var repoWeapons = await _sanctionedWeaponsRepository.GetAllSanctionedWeapons();
                    weapons = repoWeapons.Select(a => a.Id);
                }

                if (weapons.Any())
                {
                    await _cache.SetAsync(_sanctionedWeaponsCacheKey, weapons, _sanctionedWeaponsCacheExpiration);
                }

                return weapons;
            }
            finally
            {
                _sanctionedStoreLock.Release();
            }
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

        private async Task<IEnumerable<int>> GetSanctionedWeaponsFromMasterListAsync()
        {
            try
            {
                var httpClient = new HttpClient();
                var result = await httpClient.GetAsync(_masterSanctionedWeaponsEndpoint);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to retrieve master sanctioned list from remote resource. Returned '{result.StatusCode}'.");
                    return null;
                }

                var serializedResult = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Dictionary<int, string>>(serializedResult)
                    .Where(a => a.Value == "infantry")
                    .Select(a => a.Key);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve master sanctioned list: {ex.Message}");
            }

            return null;
        }
    }
}
