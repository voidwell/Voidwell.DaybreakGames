using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WeaponService : IWeaponService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly CensusItem _censusItem;

        public WeaponService(PS2DbContext ps2DbContext, CensusItem censusItem)
        {
            _ps2DbContext = ps2DbContext;
            _censusItem = censusItem;
        }

        public async Task<WeaponInfoResult> GetWeaponInfo(string weaponItemId)
        {
            var info = await _censusItem.GetWeaponInfo(weaponItemId);

            var hipModes = info.FireMode.Where(m => m.Type == "primary");
            var aimModes = info.FireMode.Where(m => m.Type == "secondary");

            return new WeaponInfoResult
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
                HipAcc = new AccuracyState
                {
                    Crouching = hipModes.First().States.First(s => s.PlayerState == "Crouching").MinConeOfFire,
                    CrouchWalking = hipModes.First().States.First(s => s.PlayerState == "CrouchWalking").MinConeOfFire,
                    Standing = hipModes.First().States.First(s => s.PlayerState == "Standing").MinConeOfFire,
                    Running = hipModes.First().States.First(s => s.PlayerState == "Running").MinConeOfFire,
                    Cof = hipModes.First().CofRecoil
                },
                AimAcc = new AccuracyState
                {
                    Crouching = aimModes.First().States.First(s => s.PlayerState == "Crouching").MinConeOfFire,
                    CrouchWalking = aimModes.First().States.First(s => s.PlayerState == "CrouchWalking").MinConeOfFire,
                    Standing = aimModes.First().States.First(s => s.PlayerState == "Standing").MinConeOfFire,
                    Running = aimModes.First().States.First(s => s.PlayerState == "Running").MinConeOfFire,
                    Cof = aimModes.First().CofRecoil
                },
                IronSightZoom = aimModes.First().DefaultZoom,
                FireModes = hipModes.Select(m => m.Description.English)
            };
        }

        public async Task<IEnumerable<WeaponLeaderboardRow>> GetLeaderboard(string weaponItemId, string sortColumn = "Kills", SortDirection sortDirection = SortDirection.Descending, int rowStart = 0, int limit = 250)
        {
            var weaponStats = await _ps2DbContext.CharacterWeaponStats.Where(s => s.ItemId == weaponItemId)
                .Include(i => i.Character)
                .OrderBy(sortColumn, sortDirection)
                .Skip(rowStart)
                .Take(limit)
                .ToListAsync();

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
                Kills = model.Kills,
                Deaths = model.Deaths,
                Headshots = model.Headshots,
                ShotsFired = model.FireCount,
                ShotsHit = model.HitCount,
                PlayTime = model.PlayTime,
                Score = model.Score,
                VehicleKills = model.VehicleKills,
                KillDeathRatio = model.Kills / model.Deaths,
                HeadshotRatio = model.Headshots / model.Kills,
                Accuracy = model.HitCount / model.FireCount,
                ScorePerMinute = model.Score / (model.PlayTime / 60),
                KillsPerHour = model.Kills / (model.PlayTime / 3600),
                VehicleKillsPerHour = model.VehicleKills / (model.PlayTime / 3600)
            };
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
