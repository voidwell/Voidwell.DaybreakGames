using System.Text.Json;
using AsyncKeyedLock;
using Microsoft.Extensions.Logging;
using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using static Voidwell.DaybreakGames.Census.Models.Extensions.CensusWeaponInfoModelExtensions;

namespace Voidwell.DaybreakGames.Services.Planetside;

public class WeaponService : IWeaponService
{
    private readonly ISanctionedWeaponsRepository _sanctionedWeaponsRepository;
    private readonly IWorldEventsService _worldEventsService;
    private readonly IItemService _itemService;
    private readonly IFactionStore _factionStore;
    private readonly ICache _cache;
    private readonly ILogger _logger;

    private const string _weaponInfoCacheKey = "ps2.weaponinfo";
    private const string _sanctionedWeaponsCacheKey = "ps2.sanctionedWeapons";
    private const string _masterSanctionedWeaponsEndpoint = "https://raw.githubusercontent.com/cooltrain7/Planetside-2-API-Tracker/master/Weapons/sanction-list-machine.json";
    private readonly TimeSpan _weaponInfoCacheExpiration = TimeSpan.FromHours(8);
    private readonly TimeSpan _sanctionedWeaponsCacheExpiration = TimeSpan.FromHours(8);

    private readonly AsyncKeyedLocker<string> _oracleStatLock = new();
    private readonly SemaphoreSlim _sanctionedStoreLock = new(1);

    public WeaponService(ISanctionedWeaponsRepository sanctionedWeaponRepository, IWorldEventsService worldEventsService,
        IItemService itemService, IFactionStore factionStore, ICache cache, ILogger<WeaponService> logger)
    {
        _sanctionedWeaponsRepository = sanctionedWeaponRepository;
        _worldEventsService = worldEventsService;
        _itemService = itemService;
        _factionStore = factionStore;
        _cache = cache;
        _logger = logger;
    }

    public async Task<WeaponInfoResult?> GetWeaponInfo(int weaponItemId)
    {
        return await _cache.GetOrSetIfNotNullAsync<WeaponInfoResult>($"{_weaponInfoCacheKey}_{weaponItemId}", async ct =>
        {
            var info = await _itemService.GetWeaponInfoAsync(weaponItemId);
            if (info == null)
            {
                return null;
            }

            Faction faction = null!;
            if (info.FactionId != null)
            {
                faction = (await _factionStore.GetFactionByIdAsync(info.FactionId.Value))!;
            }

            var hipModes = info.GetFireModesOfType(FireModeType.Primary)?.ToList();
            var aimModes = info.GetFireModesOfType(FireModeType.Secondary)?.ToList();

            var weaponInfo = new WeaponInfoResult
            {
                Name = info.GetName(),
                ItemId = weaponItemId,
                Category = info.GetCategory(),
                FactionId = info.FactionId,
                FactionName = faction?.Name,
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
                FireModes = hipModes?.GetFireModeNames()!,
                IsVehicleWeapon = info.IsVehicleWeapon,
                DamageRadius = info.GetDamageRadius(),
                HipAcc = GetAccuracyStateFromFireMode(hipModes!),
                AimAcc = GetAccuracyStateFromFireMode(aimModes!)
            };

            return weaponInfo;
        }, _weaponInfoCacheExpiration);
    }

    public async Task<WeaponInfoResult?> GetWeaponInfoByName(string weaponName)
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

    private static AccuracyState? GetAccuracyStateFromFireMode(IEnumerable<CensusWeaponInfoModel.WeaponFireMode> modes)
    {
        if (modes?.Any() != true)
        {
            return null;
        }

        var mode = modes.FirstOrDefault();

        return new AccuracyState
        {
            Crouching = mode!.GetFireModeStateCoF(FireModeState.Crouching),
            CrouchWalking = mode!.GetFireModeStateCoF(FireModeState.CrouchWalking),
            Standing = mode!.GetFireModeStateCoF(FireModeState.Standing),
            Running = mode!.GetFireModeStateCoF(FireModeState.Running),
            Cof = mode!.CofRecoil
        };
    }

    public async Task<IEnumerable<int>> GetAllSanctionedWeaponIds()
    {
        await _sanctionedStoreLock.WaitAsync();

        try
        {
            return await _cache.GetOrSetAsync<IEnumerable<int>>(
                _sanctionedWeaponsCacheKey,
                async ct =>
                {
                    var weapons = await GetSanctionedWeaponsFromMasterListAsync();
                    if (weapons == null)
                    {
                        var repoWeapons = await _sanctionedWeaponsRepository.GetAllSanctionedWeapons();
                        weapons = repoWeapons.Select(a => a.Id).ToList();
                    }

                    return weapons;
                },
                _sanctionedWeaponsCacheExpiration,
                weapons => weapons.Any());
        }
        finally
        {
            _sanctionedStoreLock.Release();
        }
    }

    public async Task<Dictionary<int, IEnumerable<DailyWeaponStats>>> GetOracleStatsFromWeaponByDateAsync(IEnumerable<int> weaponIds, DateTime start, DateTime end)
    {
        var statTasks = weaponIds.Select(id => GetOracleStatsAsync(id, start, end)).ToArray();
        await Task.WhenAll(statTasks);

        var oracleDict = new Dictionary<int, IEnumerable<DailyWeaponStats>>();
        for (var i = 0; i < weaponIds.Count(); i++)
        {
            oracleDict[weaponIds.ToArray()[i]] = statTasks.ToArray()[i].Result!;
        }

        return oracleDict;
    }

    private async Task<IEnumerable<DailyWeaponStats>?> GetOracleStatsAsync(int weaponId, DateTime start, DateTime end)
    {
        var cacheKey = $"ps2.oracle_{weaponId}_{start.Year}-{start.Month}-{start.Day}_{end.Year}-{end.Month}-{end.Day}";

        using (await _oracleStatLock.LockAsync(cacheKey))
        {
            return await _cache.GetOrSetAsync<IEnumerable<DailyWeaponStats>>(cacheKey, ct => _worldEventsService.GetDailyWeaponAggregatesByWeaponIdAsync(weaponId, start, end), TimeSpan.FromHours(1));
        }
    }

    private async Task<IEnumerable<int>?> GetSanctionedWeaponsFromMasterListAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            using var result = await httpClient.GetAsync(_masterSanctionedWeaponsEndpoint);
            if (!result.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to retrieve master sanctioned list from remote resource. Returned '{StatusCode}'.", result.StatusCode);
                return null;
            }

            var serializedResult = await result.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<int, string>>(serializedResult)!
                .Where(a => a.Value == "infantry")
                .Select(a => a.Key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve master sanctioned list");
        }

        return null;
    }
}
