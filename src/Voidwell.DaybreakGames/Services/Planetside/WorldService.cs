using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
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
        private readonly TimeSpan _activityCacheExpiration = TimeSpan.FromMinutes(1);

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
                /*
                var activity = await _cache.GetAsync<WorldActivity>(cacheKey);
                if (activity != null)
                {
                    return activity;
                }
                */

                var activity = new WorldActivity();

                var combatStatsTask = _combatReportService.GetCombatStats(worldId, startDate, endDate);
                var experienceTask = GetWorldActivityExperience(worldId, startDate, endDate);

                await Task.WhenAll(combatStatsTask, experienceTask);

                var combatStats = combatStatsTask.Result;

                activity.Stats = CreateWorldActivityStats(combatStats.Participants);
                activity.ClassStats = combatStats.Classes.OrderBy(a => a.Profile.TypeId);
                activity.TopVehicles = combatStats.Vehicles.OrderByDescending(a => a.Kills).Where(a => a.Kills > 0).Take(20);
                activity.TopPlayers = combatStats.Participants.OrderByDescending(a => a.Kills).Take(25);
                activity.TopOutfits = combatStats.Outfits.Where(a => a.ParticipantCount > 4).OrderByDescending(a => a.Kills / a.ParticipantCount).Take(10);
                activity.TopWeapons = combatStats.Weapons.OrderByDescending(a => a.Kills).Take(20);
                activity.TopExperience = experienceTask.Result;

                await _cache.SetAsync(cacheKey, activity, _activityCacheExpiration);

                return activity;
            }
        }

        private WorldActivityStats CreateWorldActivityStats(IEnumerable<CombatReportParticipantStats> participants)
        {
            var stats = new WorldActivityStats();

            var vsPlayers = participants.Where(a => a.Character.FactionId == 1);
            var ncPlayers = participants.Where(a => a.Character.FactionId == 2);
            var trPlayers = participants.Where(a => a.Character.FactionId == 3);
            var nsPlayers = participants.Where(a => a.Character.FactionId == 4);

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
            return new WorldActivityExperienceFactions
            {
                VS = items.Where(a => a.CharacterFactionId == 1).Take(5),
                NC = items.Where(a => a.CharacterFactionId == 2).Take(5),
                TR = items.Where(a => a.CharacterFactionId == 3).Take(5),
                NS = items.Where(a => a.CharacterFactionId == 4).Take(5)
            };
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
    }
}
