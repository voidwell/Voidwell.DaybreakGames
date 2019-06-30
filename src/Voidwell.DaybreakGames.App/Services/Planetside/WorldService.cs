using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.App.Models;
using Voidwell.DaybreakGames.CensusServices;
using Voidwell.DaybreakGames.CensusServices.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class WorldService : IWorldService
    {
        private readonly IWorldRepository _worldRepository;
        private readonly ICombatReportService _combatReportService;
        private readonly IWorldEventsService _worldEventsService;
        private readonly ICharacterService _characterService;
        private readonly CensusWorld _censusWorld;
        private readonly ICache _cache;

        public string ServiceName => "WorldService";
        public TimeSpan UpdateInterval => TimeSpan.FromDays(31);

        private const string _cacheKeyPrefix = "ps2.worldService";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _activityCacheExpiration = TimeSpan.FromSeconds(15);

        private readonly TimeSpan _activityPopulationOffset = TimeSpan.FromHours(12);
        private readonly TimeSpan _activityPopulationPeriodInterval = TimeSpan.FromSeconds(16);

        private readonly KeyedSemaphoreSlim _worldPopulationLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _activityPopulationLock = new KeyedSemaphoreSlim();

        public WorldService(IWorldRepository worldRepository, ICombatReportService combatReportService, IWorldEventsService worldEventsService, ICharacterService characterService, CensusWorld censusWorld, ICache cache)
        {
            _worldRepository = worldRepository;
            _combatReportService = combatReportService;
            _worldEventsService = worldEventsService;
            _characterService = characterService;
            _censusWorld = censusWorld;
            _cache = cache;
        }

        public async Task<IEnumerable<World>> GetAllWorlds()
        {
            var worlds = await _cache.GetAsync<IEnumerable<World>>(_cacheKeyPrefix);
            if (worlds != null)
            {
                return worlds;
            }

            worlds = await _worldRepository.GetAllWorldsAsync();
            if (worlds != null)
            {
                await _cache.SetAsync(_cacheKeyPrefix, worlds, _cacheExpiration);
            }

            return worlds;
        }

        public async Task<World> GetWorld(int worldId)
        {
            var worlds = await GetAllWorlds();
            return worlds.FirstOrDefault(a => a.Id == worldId);
        }

        public async Task<Dictionary<int, IEnumerable<DailyPopulation>>> GetWorldPopulationHistory(IEnumerable<int> worldIds, DateTime start, DateTime end)
        {
            var popTasks = worldIds.Select(id => GetWorldPopulationHistory(id, start, end)).ToArray();
            await Task.WhenAll(popTasks);

            var popDict = new Dictionary<int, IEnumerable<DailyPopulation>>();
            for (var i = 0; i < worldIds.Count(); i++)
            {
                popDict[worldIds.ToArray()[i]] = popTasks.ToArray()[i].Result;
            }

            return popDict;
        }

        public async Task<WorldActivity> GetWorldActivity(int worldId, int periodHours)
        {
            var startDate = DateTime.UtcNow.AddHours(periodHours * -1);
            var endDate = DateTime.UtcNow;

            var cacheKey = $"{_cacheKeyPrefix}_{worldId}-hours:{periodHours}";

            using (await _activityPopulationLock.WaitAsync(cacheKey))
            {
                var cachedActivity = await _cache.GetAsync<WorldActivity>(cacheKey);
                if (cachedActivity != null)
                {
                    return cachedActivity;
                }

                var activity = new WorldActivity
                {
                    ActivityPeriodStart = startDate,
                    ActivityPeriodEnd = endDate
                };

                var combatStatsTask = _combatReportService.GetCombatStats(worldId, startDate, endDate);
                var experienceTask = GetWorldActivityExperience(worldId, startDate, endDate);
                var playerSessionsTask = GetPlayerSessions(worldId, startDate, endDate);

                await Task.WhenAll(combatStatsTask, experienceTask, playerSessionsTask);

                var combatStats = combatStatsTask.Result;
                var playerSessions = playerSessionsTask.Result;

                var topPlayers = combatStats.Participants.OrderByDescending(a => a.Kills).Take(25).ToList();

                var sessionsDic = playerSessions.GroupBy(a => a.CharacterId)
                    .ToDictionary(a => a.Key, a => a.FirstOrDefault());

                var sessionKillsTasks = topPlayers.Select(a =>
                {
                    if (sessionsDic.TryGetValue(a.Character.Id, out var session) && session.LoginDate.HasValue)
                    {
                        return GetKillCountByCharacterIdAsync(a.Character.Id, session.LoginDate.Value,
                            session.LogoutDate);
                    }

                    return Task.FromResult(0);
                });

                var sessionKillsResults = await Task.WhenAll(sessionKillsTasks);

                for (var i = 0; i < topPlayers.Count; i++)
                {
                    var player = topPlayers[i];

                    player.SessionKills = sessionKillsResults[i];

                    if (sessionsDic.TryGetValue(player.Character.Id, out var session))
                    {
                        player.LoginDate = session.LoginDate;
                        player.LogoutDate = session.LogoutDate;
                    }
                }

                activity.Stats = CreateWorldActivityStats(combatStats.Participants);
                activity.ClassStats = combatStats.Classes.OrderBy(a => a.Profile.ProfileTypeId);
                activity.TopVehicles = combatStats.Vehicles.OrderByDescending(a => a.Kills).Where(a => a.Kills > 0).Take(20);
                activity.TopPlayers = topPlayers;
                activity.TopOutfits = combatStats.Outfits.Where(a => a.ParticipantCount > 4).OrderByDescending(a => a.Kills / a.ParticipantCount).Take(10);
                activity.TopWeapons = combatStats.Weapons.OrderByDescending(a => a.Kills).Take(20);
                activity.HistoricalPopulations = GetPopulationPeriods(playerSessions, startDate, endDate);
                activity.TopExperience = experienceTask.Result;

                await _cache.SetAsync(cacheKey, activity, _activityCacheExpiration);

                return activity;
            }
        }
        
        private WorldActivityStats CreateWorldActivityStats(IEnumerable<CombatReportParticipantStats> participants)
        {
            var stats = new WorldActivityStats();

            var factionGroups = participants.Where(a => a.Character?.FactionId != null).GroupBy(a => a.Character.FactionId).ToDictionary(a => a.Key, a => a.ToList());

            var vsPlayers = factionGroups.GetValueOrDefault(1) ?? Enumerable.Empty<CombatReportParticipantStats>();
            var ncPlayers = factionGroups.GetValueOrDefault(2) ?? Enumerable.Empty<CombatReportParticipantStats>();
            var trPlayers = factionGroups.GetValueOrDefault(3) ?? Enumerable.Empty<CombatReportParticipantStats>();
            var nsPlayers = factionGroups.GetValueOrDefault(4) ?? Enumerable.Empty<CombatReportParticipantStats>();

            stats.Kills.VS = vsPlayers.Sum(a => a.Kills);
            stats.Kills.NC = ncPlayers.Sum(a => a.Kills);
            stats.Kills.TR = trPlayers.Sum(a => a.Kills);
            stats.Kills.NS = nsPlayers.Sum(a => a.Kills);

            stats.Deaths.VS = vsPlayers.Sum(a => a.Deaths);
            stats.Deaths.NC = ncPlayers.Sum(a => a.Deaths);
            stats.Deaths.TR = trPlayers.Sum(a => a.Deaths);
            stats.Deaths.NS = nsPlayers.Sum(a => a.Deaths);

            stats.Headshots.VS = vsPlayers.Sum(a => a.Headshots);
            stats.Headshots.NC = ncPlayers.Sum(a => a.Headshots);
            stats.Headshots.TR = trPlayers.Sum(a => a.Headshots);
            stats.Headshots.NS = nsPlayers.Sum(a => a.Headshots);

            stats.Suicides.VS = vsPlayers.Sum(a => a.Suicides);
            stats.Suicides.NC = ncPlayers.Sum(a => a.Suicides);
            stats.Suicides.TR = trPlayers.Sum(a => a.Suicides);
            stats.Suicides.NS = nsPlayers.Sum(a => a.Suicides);

            stats.TeamKills.VS = vsPlayers.Sum(a => a.Teamkills);
            stats.TeamKills.NC = ncPlayers.Sum(a => a.Teamkills);
            stats.TeamKills.TR = trPlayers.Sum(a => a.Teamkills);
            stats.TeamKills.NS = nsPlayers.Sum(a => a.Teamkills);

            stats.VehicleKills.VS = vsPlayers.Sum(a => a.VehicleKills);
            stats.VehicleKills.NC = ncPlayers.Sum(a => a.VehicleKills);
            stats.VehicleKills.TR = trPlayers.Sum(a => a.VehicleKills);
            stats.VehicleKills.NS = nsPlayers.Sum(a => a.VehicleKills);

            stats.KDR.VS = stats.Kills.VS / (stats.Deaths.VS > 0 ? stats.Deaths.VS : 1);
            stats.KDR.NC = stats.Kills.NC / (stats.Deaths.NC > 0 ? stats.Deaths.NC : 1);
            stats.KDR.TR = stats.Kills.TR / (stats.Deaths.TR > 0 ? stats.Deaths.TR : 1);
            stats.KDR.NS = stats.Kills.NS / (stats.Deaths.NS > 0 ? stats.Deaths.NS : 1);

            stats.HSR.VS = stats.Headshots.VS / (stats.Kills.VS > 0 ? stats.Kills.VS : 1);
            stats.HSR.NC = stats.Headshots.NC / (stats.Kills.NC > 0 ? stats.Kills.NC : 1);
            stats.HSR.TR = stats.Headshots.TR / (stats.Kills.TR > 0 ? stats.Kills.TR : 1);
            stats.HSR.NS = stats.Headshots.NS / (stats.Kills.NS > 0 ? stats.Kills.NS : 1);

            return stats;
        }

        private async Task<WorldActivityExperience> GetWorldActivityExperience(int worldId, DateTime start, DateTime end)
        {
            var reviveEventsTask = _worldEventsService.GetReviveExperienceEventsByDateAsync(worldId, start, end);
            var healEventsTask = _worldEventsService.GetHealExperienceEventsByDateAsync(worldId, start, end);
            var roadkillEventsTask = _worldEventsService.GetRoadkillExperienceEventsByDateAsync(worldId, start, end);
            var squadBeaconKillEventsTask = _worldEventsService.GetSquadBeaconKillExperienceEventsByDateAsync(worldId, start, end);

            await Task.WhenAll(reviveEventsTask, healEventsTask, roadkillEventsTask, squadBeaconKillEventsTask);

            var reviveEvents = ConvertToWorldActivityExperienceItem(reviveEventsTask.Result);
            var healEvents = ConvertToWorldActivityExperienceItem(healEventsTask.Result);
            var roadkillEvents = ConvertToWorldActivityExperienceItem(roadkillEventsTask.Result);
            var squadBeaconKillEvents = ConvertToWorldActivityExperienceItem(squadBeaconKillEventsTask.Result);

            var characterIds = reviveEvents.Select(a => a.CharacterId).ToList();
            characterIds.AddRange(healEvents.Select(a => a.CharacterId));
            characterIds.AddRange(roadkillEvents.Select(a => a.CharacterId));
            characterIds.AddRange(squadBeaconKillEvents.Select(a => a.CharacterId));

            var distinctCharacterIds = characterIds.Distinct();

            var characters = await _characterService.FindCharacters(distinctCharacterIds);

            foreach(var character in characters)
            {
                reviveEvents.Where(a => a.CharacterId == character.Id).ToList().ForEach(a => SetCharacterOnExperienceItem(character, a));
                healEvents.Where(a => a.CharacterId == character.Id).ToList().ForEach(a => SetCharacterOnExperienceItem(character, a));
                roadkillEvents.Where(a => a.CharacterId == character.Id).ToList().ForEach(a => SetCharacterOnExperienceItem(character, a));
                squadBeaconKillEvents.Where(a => a.CharacterId == character.Id).ToList().ForEach(a => SetCharacterOnExperienceItem(character, a));
            }

            return new WorldActivityExperience
            {
                Revives = ConvertToWorldActivityExperienceFactions(reviveEvents),
                Heals = ConvertToWorldActivityExperienceFactions(healEvents),
                Roadkills = ConvertToWorldActivityExperienceFactions(roadkillEvents),
                SquadBeaconKills = ConvertToWorldActivityExperienceFactions(squadBeaconKillEvents)
            };
        }

        private void SetCharacterOnExperienceItem(Character character, WorldActivityExperienceItem item)
        {
            item.CharacterName = character.Name;
            item.CharacterBattleRank = character.BattleRank;
            item.CharacterFactionId = character.FactionId;
            item.CharacterPrestigeLevel = character.PrestigeLevel;
            item.CharacterOutfitAlias = character.OutfitMembership?.Outfit?.Alias;
        }

        private List<WorldActivityExperienceItem> ConvertToWorldActivityExperienceItem(IEnumerable<GainExperience> events)
        {
            return events
                .GroupBy(a => a.CharacterId)
                .Select(a => new WorldActivityExperienceItem(a.Key, a.Count()))
                .OrderByDescending(a => a.Ticks)
                .ToList();
        }

        private WorldActivityExperienceFactions ConvertToWorldActivityExperienceFactions(IEnumerable<WorldActivityExperienceItem> items)
        {
            var result = new WorldActivityExperienceFactions();

            foreach (var item in items.GroupBy(a => a.CharacterFactionId))
            {
                var topStats = item.Take(5);

                switch (item.Key)
                {
                    case 1: { result.VS = topStats; break; }
                    case 2: { result.NC = topStats; break; }
                    case 3: { result.TR = topStats; break; }
                    case 4: { result.NS = topStats; break; }
                }
            }

            return result;
        }

        private async Task<IEnumerable<DailyPopulation>> GetWorldPopulationHistory(int worldId, DateTime start, DateTime end)
        {
            var cacheKey = $"{_cacheKeyPrefix}_{worldId}_{start.Year}-{start.Month}-{start.Day}_{end.Year}-{end.Month}-{end.Day}";

            using (await _worldPopulationLock.WaitAsync(cacheKey))
            {

                var populations = await _cache.GetAsync<IEnumerable<DailyPopulation>>(cacheKey);
                if (populations != null)
                {
                    return populations;
                }

                populations = await _worldRepository.GetDailyPopulationsByWorldIdAsync(worldId);
                if (populations != null)
                {
                    await _cache.SetAsync(cacheKey, populations, _cacheExpiration);
                }

                return populations;
            }
        }

        public async Task RefreshStore()
        {
            var worlds = await _censusWorld.GetAllWorlds();

            if (worlds != null)
            {
                await _worldRepository.UpsertRangeAsync(worlds.Select(ConvertToDbModel));
                await _cache.RemoveAsync(_cacheKeyPrefix);
            }
        }

        private World ConvertToDbModel(CensusWorldModel censusModel)
        {
            return new World
            {
                Id = censusModel.WorldId,
                Name = censusModel.Name.English
            };
        }

        private async Task<IEnumerable<PlayerActivitySession>> GetPlayerSessions(int worldId, DateTime startDate, DateTime endDate)
        {
            var populationStartDate = startDate.Add(-_activityPopulationOffset);

            var playerLoginEventsTask = _worldEventsService.GetPlayerLoginEventsAsync(worldId, populationStartDate, endDate);
            var playerLogoutEventsTask = _worldEventsService.GetPlayerLogoutEventsAsync(worldId, populationStartDate, endDate);

            await Task.WhenAll(playerLoginEventsTask, playerLogoutEventsTask);

            var loginEvents = playerLoginEventsTask.Result.OrderBy(a => a.Timestamp).GroupBy(a => a.CharacterId).ToDictionary(a => a.Key, a => a.ToList());
            var logoutEvents = playerLogoutEventsTask.Result.OrderBy(a => a.Timestamp).GroupBy(a => a.CharacterId).ToDictionary(a => a.Key, a => a.ToList());

            var sessions = new List<PlayerActivitySession>();
            foreach (var characterId in loginEvents.Keys)
            {
                sessions.AddRange(loginEvents[characterId]
                    .Select(loginEvent => new PlayerActivitySession
                    {
                        CharacterId = loginEvent.CharacterId,
                        LoginDate = loginEvent.Timestamp,
                        LogoutDate = logoutEvents.GetValueOrDefault(characterId)?.FirstOrDefault(a => a.Timestamp >= loginEvent.Timestamp)?.Timestamp
                    }));
            }

            var periodSessions = sessions.OrderByDescending(a => a.LoginDate)
                .GroupBy(a => a.CharacterId)
                .SelectMany(a => MergeSessionReconnects(a.ToList()))
                .Where(a => a.LogoutDate == null || a.LogoutDate >= startDate)
                .ToList();

            var distinctCharacterIds = periodSessions.Select(a => a.CharacterId).Distinct();
            var characterFactions = (await _characterService.FindCharacters(distinctCharacterIds))?.ToDictionary(a => a.Id, a => a.FactionId);

            if (characterFactions != null)
            {
                periodSessions.ForEach(a => a.FactionId = characterFactions.ContainsKey(a.CharacterId) ? characterFactions[a.CharacterId] : (int?)null);
            }

            return periodSessions;
        }

        private IEnumerable<PopulationPeriod> GetPopulationPeriods(IEnumerable<PlayerActivitySession> sessions, DateTime startDate, DateTime endDate)
        {
            var populationPeriods = new List<PopulationPeriod>();
            
            for (var i = startDate; i < endDate; i = i.Add(_activityPopulationPeriodInterval))
            {
                var relativeUpper = i.Add(_activityPopulationPeriodInterval);

                var relativeSessions =
                    sessions.Where(a => a.LoginDate <= relativeUpper && (a.LogoutDate >= relativeUpper || a.LogoutDate == null)).ToList();

                var vsCount = relativeSessions.Count(a => a.FactionId == 1);
                var ncCount = relativeSessions.Count(a => a.FactionId == 2);
                var trCount = relativeSessions.Count(a => a.FactionId == 3);
                var nsCount = relativeSessions.Count(a => a.FactionId == 4);

                populationPeriods.Add(new PopulationPeriod(relativeUpper, vsCount, ncCount, trCount, nsCount));
            }

            return populationPeriods.OrderByDescending(a => a.Timestamp).ToList();
        }

        private async Task<int> GetKillCountByCharacterIdAsync(string characterId, DateTime startDate, DateTime? endDate)
        {
            var characterDeathEvents = await _worldEventsService.GetDeathEventsForCharacterIdByDateAsync(characterId, startDate, endDate);

            return characterDeathEvents.Count(a => a.AttackerCharacterId == characterId && a.AttackerCharacterId != a.CharacterId && a.AttackerCharacter?.FactionId != a.Character?.FactionId);
        }

        private IEnumerable<PlayerActivitySession> MergeSessionReconnects(IEnumerable<PlayerActivitySession> sessions)
        {
            PlayerActivitySession currentSession = null;
            var uniqueSessions = new List<PlayerActivitySession>();

            foreach (var session in sessions.OrderByDescending(a => a.LoginDate))
            {
                if (currentSession == null)
                {
                    currentSession = session;
                }
                else if (currentSession.LoginDate - session.LogoutDate <= TimeSpan.FromMinutes(5))
                {
                    currentSession.LoginDate = session.LoginDate;
                }
                else
                {
                    uniqueSessions.Add(currentSession);
                    currentSession = session;
                }
            }

            uniqueSessions.Add(currentSession);

            return uniqueSessions;
        }
    }
}
