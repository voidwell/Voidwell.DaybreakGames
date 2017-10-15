using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.DBContext;
using System;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Microsoft.EntityFrameworkCore;
using Voidwell.Cache;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CharacterService : ICharacterService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly CensusCharacter _censusCharacter;
        private readonly ICache _cache;
        private readonly IOutfitService _outfitService;

        public CharacterService(PS2DbContext ps2DbContext, CensusCharacter censusCharacter, ICache cache, IOutfitService outfitService)
        {
            _ps2DbContext = ps2DbContext;
            _censusCharacter = censusCharacter;
            _cache = cache;
            _outfitService = outfitService;
        }

        public async Task<IEnumerable<CharacterSearchResult>> LookupCharactersByName(string query, int limit = 12)
        {
            var characterList = await _censusCharacter.LookupCharactersByName(query, limit);
            return characterList.Select(c => CharacterSearchResult.LoadFromCensusCharacter(c));
        }

        public async Task<IEnumerable<DbCharacter>> FindCharacters(IEnumerable<string> characterIds)
        {
            return await _ps2DbContext.Characters.Where(c => characterIds.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<DbCharacter> GetCharacter(string characterId)
        {
            var character = await _ps2DbContext.Characters.SingleAsync(c => c.Id == characterId);

            return character ?? await UpdateCharacter(characterId);
        }

        public async Task<DbCharacter> GetCharacterFull(string characterId)
        {
            var character = await TryGetCharacterFull(characterId);

            if (character != null)
            {
                return character;
            }

            await UpdateCharacter(characterId, null);

            return await TryGetCharacterFull(characterId);
        }

        public async Task UpdateCharacter(string characterId, DateTime? lastLoginDate = null)
        {
            var character = await UpdateCharacter(characterId);

            if (character == null)
            {
                return;
            }

            await Task.WhenAll(UpdateCharacterTimes(characterId),
                               UpdateCharacterStats(characterId, lastLoginDate),
                               UpdateCharacterWeaponStats(characterId, lastLoginDate),
                               UpdateCharacterOutfitMembership(characterId));
        }

        public Task<DbOutfitMember> GetCharactersOutfit(string characterId)
        {
            return UpdateCharacterOutfitMembership(characterId);
        }

        private Task<DbCharacter> TryGetCharacterFull(string characterId)
        {
            return _ps2DbContext.Characters
                .Where(c => c.Id == characterId)
                .Include(c => c.Title)
                .Include(c => c.World)
                .Include(c => c.Faction)
                .Include(c => c.Time)
                .Include(c => c.OutfitMembership)
                    .ThenInclude(m => m.Outfit)
                .Include(c => c.Stats)
                    .ThenInclude(s => s.Profile)
                .Include(c => c.StatsByFaction)
                    .ThenInclude(s => s.Profile)
                .Include(c => c.WeaponStats)
                    .ThenInclude(s => s.Item)
                        .ThenInclude(i => i.ItemCategory)
                .Include(c => c.WeaponStats)
                    .ThenInclude(s => s.Vehicle)
                .FirstOrDefaultAsync();
        }

        private async Task<DbCharacter> UpdateCharacter(string characterId)
        {
            var character = await _censusCharacter.GetCharacter(characterId);

            if (character == null)
            {
                return null;
            }

            var dataModel = new DbCharacter
            {
                Id = character.CharacterId,
                Name = character.Name.First,
                FactionId = character.FactionId,
                WorldId = character.WorldId,
                BattleRank = character.BattleRank.Value,
                BattleRankPercentToNext = character.BattleRank.PercentToNext,
                CertsEarned = character.Certs.EarnedPoints,
                TitleId = character.TitleId
            };

            _ps2DbContext.Update(dataModel);
            await _ps2DbContext.SaveChangesAsync();

            await UpdateCharacterOutfitMembership(characterId);

            return dataModel;
        }

        private async Task UpdateCharacterTimes(string characterId)
        {
            var times = await _censusCharacter.GetCharacterTimes(characterId);

            if (times == null)
            {
                return;
            }

            var dataModel = new DbCharacterTime
            {
                CharacterId = characterId,
                CreatedDate = times.Creation,
                LastLoginDate = times.LastLogin,
                LastSaveDate = times.LastSave,
                MinutesPlayed = times.MinutesPlayed
            };

            _ps2DbContext.Update(dataModel);
            await _ps2DbContext.SaveChangesAsync();
        }

        private async Task UpdateCharacterStats(string characterId, DateTime? LastLoginDate)
        {
            var characterStatsTask = _censusCharacter.GetCharacterStats(characterId, LastLoginDate);
            var characterStatsByFactionTask = _censusCharacter.GetCharacterFactionStats(characterId, LastLoginDate);

            await Task.WhenAll(characterStatsTask, characterStatsByFactionTask);

            var stats = characterStatsTask.Result;
            var statsByFaction = characterStatsByFactionTask.Result;

            if (stats == null && statsByFaction == null)
            {
                return;
            }

            var storedStats = _ps2DbContext.CharacterStats.Where(s => stats.Any(c => c.CharacterId == s.CharacterId && c.ProfileId == s.ProfileId)).AsTracking();
            var storedStatsByFaction = _ps2DbContext.CharacterStatByFactions.Where(s => statsByFaction.Any(c => c.CharacterId == s.CharacterId && c.ProfileId == s.ProfileId)).AsTracking();

            var newModels = new List<DbCharacterStat>();
            var newModelsByFaction = new List<DbCharacterStatByFaction>();

            foreach (var stat in stats)
            {
                var dbModel = storedStats.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ProfileId == stat.ProfileId);

                if (dbModel == null)
                {
                    dbModel = newModels.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ProfileId == stat.ProfileId);
                }

                if (dbModel != null)
                {
                    AssignStatValue(ref dbModel, stat.StatName, stat.ValueForever);
                    continue;
                }

                dbModel = new DbCharacterStat
                {
                    CharacterId = stat.CharacterId,
                    ProfileId = stat.ProfileId
                };

                AssignStatValue(ref dbModel, stat.StatName, stat.ValueForever);
                newModels.Add(dbModel);
            }

            foreach (var stat in statsByFaction)
            {
                var dbModel = storedStats.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ProfileId == stat.ProfileId);

                if (dbModel == null)
                {
                    dbModel = newModels.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ProfileId == stat.ProfileId);
                }

                var value = stat.ValueForeverVs + stat.ValueForeverNc + stat.ValueForeverTr;

                if (dbModel != null)
                {
                    AssignStatValue(ref dbModel, stat.StatName, value);
                    continue;
                }

                dbModel = new DbCharacterStat
                {
                    CharacterId = stat.CharacterId,
                    ProfileId = stat.ProfileId
                };
                
                AssignStatValue(ref dbModel, stat.StatName, value);
                newModels.Add(dbModel);
            }

            foreach (var stat in statsByFaction)
            {
                var dbModel = storedStatsByFaction.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ProfileId == stat.ProfileId);

                if (dbModel == null)
                {
                    dbModel = newModelsByFaction.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ProfileId == stat.ProfileId);
                }

                if (dbModel != null)
                {
                    AssignStatValue(ref dbModel, stat.StatName, stat.ValueForeverVs, stat.ValueForeverNc, stat.ValueForeverTr);
                    continue;
                }

                dbModel = new DbCharacterStatByFaction
                {
                    CharacterId = stat.CharacterId,
                    ProfileId = stat.ProfileId
                };

                AssignStatValue(ref dbModel, stat.StatName, stat.ValueForeverVs, stat.ValueForeverNc, stat.ValueForeverTr);
                newModelsByFaction.Add(dbModel);
            }

            _ps2DbContext.AddRange(newModels);
            _ps2DbContext.AddRange(newModelsByFaction);
            await _ps2DbContext.SaveChangesAsync();
        }

        private void AssignStatValue(ref DbCharacterStat dbModel, string statName, int value)
        {
            switch (statName)
            {
                case "achievement_count":
                    dbModel.AchievementCount = value;
                    break;
                case "assist_count":
                    dbModel.AssistCount = value;
                    break;
                case "deaths":
                    dbModel.Deaths = value;
                    break;
                case "facility_defended_count":
                    dbModel.FacilityDefendedCount = value;
                    break;
                case "fire_count":
                    dbModel.FireCount = value;
                    break;
                case "hit_count":
                    dbModel.HitCount = value;
                    break;
                case "medal_count":
                    dbModel.MedalCount = value;
                    break;
                case "play_time":
                    dbModel.PlayTime = value;
                    break;
                case "score":
                    dbModel.Score = value;
                    break;
                case "skill_points":
                    dbModel.SkillPoints = value;
                    break;
                case "weapon_deaths":
                    dbModel.WeaponDeaths = value;
                    break;
                case "weapon_fire_count":
                    dbModel.WeaponFireCount = value;
                    break;
                case "weapon_hit_count":
                    dbModel.WeaponHitCount = value;
                    break;
                case "weapon_play_time":
                    dbModel.WeaponPlayTime = value;
                    break;
                case "weapon_score":
                    dbModel.WeaponScore = value;
                    break;
                case "domination_count":
                    dbModel.DominationCount = value;
                    break;
                case "facility_capture_count":
                    dbModel.FacilityCaptureCount = value;
                    break;
                case "killed_by":
                    dbModel.KilledBy = value;
                    break;
                case "kills":
                    dbModel.Kills = value;
                    break;
                case "revenge_count":
                    dbModel.RevengeCount = value;
                    break;
                case "weapon_damage_given":
                    dbModel.WeaponDamageGiven = value;
                    break;
                case "weapon_damage_taken_by":
                    dbModel.WeaponDamageTakenBy = value;
                    break;
                case "weapon_headshots":
                    dbModel.WeaponHeadshots = value;
                    break;
                case "weapon_killed_by":
                    dbModel.WeaponKilledBy = value;
                    break;
                case "weapon_kills":
                    dbModel.Kills = value;
                    break;
                case "weapon_vehicle_kills":
                    dbModel.WeaponVehicleKills = value;
                    break;
            }
        }

        private void AssignStatValue(ref DbCharacterStatByFaction dbModel, string statName, int valueVs, int valueNc, int valueTr)
        {
            switch (statName)
            {
                case "domination_count":
                    dbModel.DominationCountVS = valueVs;
                    dbModel.DominationCountNC = valueNc;
                    dbModel.DominationCountTR = valueTr;
                    break;
                case "facility_capture_count":
                    dbModel.FacilityCaptureCountVS = valueVs;
                    dbModel.FacilityCaptureCountNC = valueNc;
                    dbModel.FacilityCaptureCountTR = valueTr;
                    break;
                case "killed_by":
                    dbModel.KilledByVS = valueVs;
                    dbModel.KilledByNC = valueNc;
                    dbModel.KilledByTR = valueTr;
                    break;
                case "kills":
                    dbModel.KillsVS = valueVs;
                    dbModel.KillsNC = valueNc;
                    dbModel.KillsTR = valueTr;
                    break;
                case "revenge_count":
                    dbModel.RevengeCountVS = valueVs;
                    dbModel.RevengeCountNC = valueNc;
                    dbModel.RevengeCountTR = valueTr;
                    break;
                case "weapon_damage_given":
                    dbModel.WeaponDamageGivenVS = valueVs;
                    dbModel.WeaponDamageGivenNC = valueNc;
                    dbModel.WeaponDamageGivenTR = valueTr;
                    break;
                case "weapon_damage_taken_by":
                    dbModel.WeaponDamageTakenByVS = valueVs;
                    dbModel.WeaponDamageTakenByNC = valueNc;
                    dbModel.WeaponDamageTakenByTR = valueTr;
                    break;
                case "weapon_headshots":
                    dbModel.WeaponHeadshotsVS = valueVs;
                    dbModel.WeaponHeadshotsNC = valueNc;
                    dbModel.WeaponHeadshotsTR = valueTr;
                    break;
                case "weapon_killed_by":
                    dbModel.WeaponKilledByVS = valueVs;
                    dbModel.WeaponKilledByNC = valueNc;
                    dbModel.WeaponKilledByTR = valueTr;
                    break;
                case "weapon_kills":
                    dbModel.WeaponKillsVS = valueVs;
                    dbModel.WeaponKillsNC = valueNc;
                    dbModel.WeaponKillsTR = valueTr;
                    break;
                case "weapon_vehicle_kills":
                    dbModel.WeaponVehicleKillsVS = valueVs;
                    dbModel.WeaponVehicleKillsNC = valueNc;
                    dbModel.WeaponVehicleKillsTR = valueTr;
                    break;
            }
        }

        private async Task UpdateCharacterWeaponStats(string characterId, DateTime? LastLoginDate)
        {
            var characterWepStatsTask = _censusCharacter.GetCharacterWeaponStats(characterId, LastLoginDate);
            var characterWepStatsByFactionTask = _censusCharacter.GetCharacterWeaponStatsByFaction(characterId, LastLoginDate);

            await Task.WhenAll(characterWepStatsTask, characterWepStatsByFactionTask);

            var wepStats = characterWepStatsTask.Result;
            var wepStatsByFaction = characterWepStatsByFactionTask.Result;

            if (wepStats == null && wepStatsByFaction == null)
            {
                return;
            }

            var storedStats = _ps2DbContext.CharacterWeaponStats.Where(s => wepStats.Any(c => c.CharacterId == s.CharacterId && c.ItemId == s.ItemId)).AsTracking();
            var storedStatsByFaction = _ps2DbContext.CharacterWeaponStatByFactions.Where(s => wepStats.Any(c => c.CharacterId == s.CharacterId && c.ItemId == s.ItemId)).AsTracking();

            var newModels = new List<DbCharacterWeaponStat>();
            var newModelsByFaction = new List<DbCharacterWeaponStatByFaction>();

            foreach (var stat in wepStats)
            {
                var dbModel = storedStats.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ItemId == stat.ItemId);

                if (dbModel == null)
                {
                    dbModel = newModels.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ItemId == stat.ItemId);
                }

                if (dbModel != null)
                {
                    AssignStatValue(ref dbModel, stat.StatName, stat.Value);
                    continue;
                }

                dbModel = new DbCharacterWeaponStat
                {
                    CharacterId = stat.CharacterId,
                    ItemId = stat.ItemId,
                    VehicleId = stat.VehicleId
                };

                AssignStatValue(ref dbModel, stat.StatName, stat.Value);
                newModels.Add(dbModel);
            }

            foreach (var stat in wepStatsByFaction)
            {
                var dbModel = storedStats.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ItemId == stat.ItemId);

                if (dbModel == null)
                {
                    dbModel = newModels.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ItemId == stat.ItemId);
                }

                var value = stat.ValueVs + stat.ValueNc + stat.ValueTr;

                if (dbModel != null)
                {
                    AssignStatValue(ref dbModel, stat.StatName, value);
                    continue;
                }

                dbModel = new DbCharacterWeaponStat
                {
                    CharacterId = stat.CharacterId,
                    ItemId = stat.ItemId,
                    VehicleId = stat.VehicleId
                };

                AssignStatValue(ref dbModel, stat.StatName, value);
                newModels.Add(dbModel);
            }

            foreach (var stat in wepStatsByFaction)
            {
                var dbModel = storedStatsByFaction.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ItemId == stat.ItemId);

                if (dbModel == null)
                {
                    dbModel = newModelsByFaction.FirstOrDefault(s => s.CharacterId == stat.CharacterId && s.ItemId == stat.ItemId);
                }

                if (dbModel != null)
                {
                    AssignStatValue(ref dbModel, stat.StatName, stat.ValueVs, stat.ValueNc, stat.ValueTr);
                    continue;
                }

                dbModel = new DbCharacterWeaponStatByFaction
                {
                    CharacterId = stat.CharacterId,
                    ItemId = stat.ItemId,
                    VehicleId = stat.VehicleId
                };

                AssignStatValue(ref dbModel, stat.StatName, stat.ValueVs, stat.ValueNc, stat.ValueTr);
                newModelsByFaction.Add(dbModel);
            }

            _ps2DbContext.AddRange(newModels);
            _ps2DbContext.AddRange(newModelsByFaction);
            await _ps2DbContext.SaveChangesAsync();
        }

        private void AssignStatValue(ref DbCharacterWeaponStat dbModel, string statName, int value)
        {
            switch (statName)
            {
                case "weapon_deaths":
                    dbModel.Deaths = value;
                    break;
                case "weapon_fire_count":
                    dbModel.FireCount = value;
                    break;
                case "weapon_hit_count":
                    dbModel.HitCount = value;
                    break;
                case "weapon_play_time":
                    dbModel.PlayTime = value;
                    break;
                case "weapon_score":
                    dbModel.Score = value;
                    break;
                case "weapon_damage_given":
                    dbModel.DamageGiven = value;
                    break;
                case "weapon_headshots":
                    dbModel.Headshots = value;
                    break;
                case "weapon_killed_by":
                    dbModel.KilledBy = value;
                    break;
                case "weapon_kills":
                    dbModel.Kills = value;
                    break;
                case "weapon_vehicle_kills":
                    dbModel.VehicleKills = value;
                    break;
            }
        }

        private void AssignStatValue(ref DbCharacterWeaponStatByFaction dbModel, string statName, int valueVs, int valueNc, int valueTr)
        {
            switch (statName)
            {
                case "weapon_damage_given":
                    dbModel.DamageGivenVS = valueVs;
                    dbModel.DamageGivenNC = valueNc;
                    dbModel.DamageGivenTR = valueTr;
                    break;
                case "weapon_headshots":
                    dbModel.HeadshotsVS = valueVs;
                    dbModel.HeadshotsNC = valueNc;
                    dbModel.HeadshotsTR = valueTr;
                    break;
                case "weapon_killed_by":
                    dbModel.KilledByVS = valueVs;
                    dbModel.KilledByNC = valueNc;
                    dbModel.KilledByTR = valueTr;
                    break;
                case "weapon_kills":
                    dbModel.KillsVS = valueVs;
                    dbModel.KillsNC = valueNc;
                    dbModel.KillsTR = valueTr;
                    break;
                case "weapon_vehicle_kills":
                    dbModel.VehicleKillsVS = valueVs;
                    dbModel.VehicleKillsNC = valueNc;
                    dbModel.VehicleKillsTR = valueTr;
                    break;
            }
        }

        private async Task<DbOutfitMember> UpdateCharacterOutfitMembership(string characterId)
        {
            var membership = await _censusCharacter.GetCharacterOutfitMembership(characterId);

            if (membership == null)
            {
                var dbMembership = await _ps2DbContext.OutfitMembers.FindAsync(characterId);
                if (dbMembership != null)
                {
                    _ps2DbContext.OutfitMembers.Remove(dbMembership);
                    await _ps2DbContext.SaveChangesAsync();
                }
                return null;
            }

            await _outfitService.GetOutfit(membership.OutfitId);

            var dataModel = new DbOutfitMember
            {
                OutfitId = membership.OutfitId,
                CharacterId = membership.CharacterId,
                MemberSinceDate = membership.MemberSinceDate,
                Rank = membership.Rank,
                RankOrdinal = membership.RankOrdinal
            };

            _ps2DbContext.Update(dataModel);
            await _ps2DbContext.SaveChangesAsync();

            return dataModel;
        }

        public async Task<IEnumerable<DbPlayerSession>> GetSessions(string characterId)
        {
            return await _ps2DbContext.PlayerSessions.Where(s => s.CharacterId == characterId && s.LogoutDate != null)
                .OrderBy("LoginDate", SortDirection.Descending)
                .Take(25)
                .ToArrayAsync();
        }

        public async Task<PlayerSession> GetSession(string characterId, string sessionId)
        {
            var session = await _ps2DbContext.PlayerSessions.Where(s => s.Id == sessionId && s.CharacterId == characterId)
                .SingleOrDefaultAsync();

            if (session == null)
            {
                return null;
            }

            var sessionDeaths = await _ps2DbContext.EventDeaths.Where(d => d.AttackerCharacterId == characterId || d.CharacterId == characterId && d.Timestamp > session.LoginDate && d.Timestamp < session.LogoutDate)
                .Include(i => i.AttackerCharacter)
                .Include(i => i.Character)
                .Include(i => i.AttackerWeapon)
                .ToArrayAsync();

            var sessionEvents = sessionDeaths.Select(d => new PlayerSessionEvent
            {
                Attacker = new CombatReportItemDetail { Id = d.AttackerCharacterId, Name = d.AttackerCharacter.Name, FactionId = d.AttackerCharacter.FactionId },
                Victim = new CombatReportItemDetail { Id = d.CharacterId, Name = d.Character.Name, FactionId = d.Character.FactionId },
                Weapon = new PlayerSessionWeapon { Id = d.AttackerWeaponId, ImageId = d.AttackerWeapon.ImageId, Name = d.AttackerWeapon.Name },
                Timestamp = d.Timestamp,
                ZoneId = d.ZoneId,
                IsHeadshot = d.IsHeadshot,
                AttackerFireModeId = d.AttackerFireModeId,
                AttackerLoadoutId = d.AttackerLoadoutId,
                AttackerOutfitId = d.AttackerOutfitId,
                AttackerVehicleId = d.AttackerVehicleId,
                CharacterLoadoutId = d.CharacterLoadoutId,
                CharacterOutfitId = d.CharacterOutfitId
            });

            return new PlayerSession
            {
                Events = sessionEvents,
                Session = new PlayerSessionInfo
                {
                    CharacterId = session.CharacterId,
                    Id = session.Id,
                    Duration = session.Duration,
                    LoginDate = session.LoginDate,
                    LogoutDate = session.LogoutDate
                }
            };
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
