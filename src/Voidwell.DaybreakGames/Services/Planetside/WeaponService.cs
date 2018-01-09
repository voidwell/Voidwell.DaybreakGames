using Newtonsoft.Json.Linq;
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
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WeaponService : IWeaponService
    {
        private readonly ICharacterRepository _characterRepository;
        private readonly CensusItem _censusItem;
        private readonly ICache _cache;

        private readonly TimeSpan _weaponInfoCacheExpiration = TimeSpan.FromHours(1);
        private readonly string _weaponInfoCacheKey = "ps2.weaponinfo";

        public WeaponService(ICharacterRepository characterRepository, CensusItem censusItem, ICache cache)
        {
            _characterRepository = characterRepository;
            _censusItem = censusItem;
            _cache = cache;
        }

        public async Task<WeaponInfoResult> GetWeaponInfo(string weaponItemId)
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

        public async Task<IEnumerable<WeaponLeaderboardRow>> GetLeaderboard(string weaponItemId, string sortColumn = "Kills", SortDirection sortDirection = SortDirection.Descending, int rowStart = 0, int limit = 250)
        {
            var weaponStats = await _characterRepository.GetCharacterWeaponLeaderboardAsync(weaponItemId, sortColumn, sortDirection, rowStart, limit);
            return weaponStats.Select(s => ConvertToResult(s));
        }

        private WeaponLeaderboardRow ConvertToResult(DbCharacterWeaponStat model)
        {
            return new WeaponLeaderboardRow
            {
                CharacterId = model.CharacterId,
                Name = model.Character.Name,
                FactionId = model.Character.FactionId,
                WorldId = model.Character.WorldId,
                Kills = model.Kills.Value,
                Deaths = model.Deaths.Value,
                Headshots = model.Headshots.Value,
                ShotsFired = model.FireCount.Value,
                ShotsHit = model.HitCount.Value,
                PlayTime = model.PlayTime.Value,
                Score = model.Score.Value,
                VehicleKills = model.VehicleKills.Value,
                KillDeathRatio = model.Kills.Value / model.Deaths.Value,
                HeadshotRatio = model.Headshots.Value / model.Kills.Value,
                Accuracy = model.HitCount.Value / model.FireCount.Value,
                ScorePerMinute = model.Score.Value / (model.PlayTime.Value / 60),
                KillsPerHour = model.Kills.Value / (model.PlayTime.Value / 3600),
                VehicleKillsPerHour = model.VehicleKills.Value / (model.PlayTime.Value / 3600)
            };
        }
    }
}
