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

        public WeaponService(PS2DbContext ps2DbContext)
        {
            _ps2DbContext = ps2DbContext;
        }

        public async Task GetWeaponInfo(string weaponItemId)
        {
            var info = await CensusItem.GetWeaponInfo(weaponItemId);

            // ToDo: create data sheet model
        }

        public async Task<IEnumerable<WeaponLeaderboardRow>> GetLeaderboard(string weaponItemId, string sortColumn, SortDirection sortDirection, int rowStart, int limit)
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
