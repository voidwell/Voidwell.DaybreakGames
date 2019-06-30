using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Models.Planetside.Events;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CombatReportService : ICombatReportService
    {
        private readonly IWorldEventsService _worldEventsService;
        private readonly ICharacterService _characterService;
        private readonly IOutfitService _outfitService;
        private readonly IItemService _itemService;
        private readonly IVehicleService _vehicleService;
        private readonly IMapService _mapService;
        private readonly IProfileService _profileService;

        public CombatReportService(IWorldEventsService worldEventsService, ICharacterService characterService,
            IOutfitService outfitService, IItemService itemService, IVehicleService vehicleService, IMapService mapService,
            IProfileService profileService)
        {
            _worldEventsService = worldEventsService;
            _characterService = characterService;
            _outfitService = outfitService;
            _itemService = itemService;
            _vehicleService = vehicleService;
            _mapService = mapService;
            _profileService = profileService;
        }

        public async Task<CombatReport> GetCombatReport(int worldId, int zoneId, DateTime startDate, DateTime? endDate)
        {
            var combatStatsTask = GetCombatStats(worldId, startDate, endDate, zoneId);
            var captureLogTask = GetCaptureLog(worldId, zoneId, startDate, endDate);

            await Task.WhenAll(combatStatsTask, captureLogTask);

            var combatReport = new CombatReport
            {
                Stats = combatStatsTask.Result,
                CaptureLog = captureLogTask.Result
            };

            return combatReport;
        }

        public async Task<CombatReportStats> GetCombatStats(int worldId, DateTime startDate, DateTime? endDate, int? zoneId = null)
        {
            var characterDeathsTask = GetCharacterDeaths(worldId, startDate, endDate, zoneId);
            var vehicleDeathsTask = GetVehicleDeaths(worldId, startDate, endDate, zoneId);
            var facilityControlsTask = _worldEventsService.GetFacilityControlsByDateAsync(worldId, startDate, endDate, zoneId);

            await Task.WhenAll(characterDeathsTask, vehicleDeathsTask, facilityControlsTask);

            var deaths = characterDeathsTask.Result.ToList();
            var vehicleDeaths = vehicleDeathsTask.Result.ToList();
            var facilityControls = facilityControlsTask.Result.ToList();

            var participantHash = new Dictionary<string, CombatReportParticipantStats>();
            var outfitHash = new Dictionary<string, CombatReportOutfitStats>();
            var vehicleHash = new Dictionary<int, CombatReportVehicleStats>();
            var weaponHash = new Dictionary<int, CombatReportWeaponStats>();
            var loadoutHash = new Dictionary<int, CombatReportClassStats>();

            PreLoadCombatStatTrackers(deaths, vehicleDeaths, participantHash, outfitHash, vehicleHash, weaponHash, loadoutHash);

            await LoadCombatStatDetails(participantHash, outfitHash, vehicleHash, weaponHash, loadoutHash);

            CalculateCombatStats(deaths, vehicleDeaths, facilityControls, participantHash, outfitHash, vehicleHash, weaponHash, loadoutHash);

            SetCombatStatTopItems(deaths, participantHash, weaponHash, loadoutHash);

            return new CombatReportStats
            {
                Participants = participantHash.Values.ToArray(),
                Outfits = outfitHash.Values.ToArray(),
                Weapons = weaponHash.Values.ToArray(),
                Vehicles = vehicleHash.Values.ToArray(),
                Classes = loadoutHash.Values.ToArray()
            };
        }

        private void PreLoadCombatStatTrackers(IEnumerable<Death> deaths, IEnumerable<VehicleDestroy> vehicleDeaths, Dictionary<string, CombatReportParticipantStats> participantHash,
            Dictionary<string, CombatReportOutfitStats> outfitHash, Dictionary<int, CombatReportVehicleStats> vehicleHash, Dictionary<int, CombatReportWeaponStats> weaponHash, 
            Dictionary<int, CombatReportClassStats> loadoutHash)
        {
            foreach (var death in deaths)
            {
                if (!participantHash.ContainsKey(death.AttackerCharacterId))
                {
                    participantHash.Add(death.AttackerCharacterId, new CombatReportParticipantStats(death.AttackerCharacterId, death.AttackerOutfitId));
                }

                if (!participantHash.ContainsKey(death.CharacterId))
                {
                    participantHash.Add(death.CharacterId, new CombatReportParticipantStats(death.CharacterId, death.CharacterOutfitId));
                }

                if (death.AttackerOutfitId != null && !outfitHash.ContainsKey(death.AttackerOutfitId))
                {
                    outfitHash.Add(death.AttackerOutfitId, new CombatReportOutfitStats(death.AttackerOutfitId));
                }

                if (death.CharacterOutfitId != null && !outfitHash.ContainsKey(death.CharacterOutfitId))
                {
                    outfitHash.Add(death.CharacterOutfitId, new CombatReportOutfitStats(death.CharacterOutfitId));
                }

                if (death.AttackerWeaponId != null && death.AttackerWeaponId != 0 && !weaponHash.ContainsKey(death.AttackerWeaponId.Value))
                {
                    weaponHash.Add(death.AttackerWeaponId.Value, new CombatReportWeaponStats(death.AttackerWeaponId.Value));
                }

                if (death.AttackerVehicleId != null && death.AttackerVehicleId != 0 && !vehicleHash.ContainsKey(death.AttackerVehicleId.Value))
                {
                    vehicleHash.Add(death.AttackerVehicleId.Value, new CombatReportVehicleStats(death.AttackerVehicleId.Value));
                }

                if (death.AttackerLoadoutId != null && !loadoutHash.ContainsKey(death.AttackerLoadoutId.Value))
                {
                    loadoutHash.Add(death.AttackerLoadoutId.Value, new CombatReportClassStats(death.AttackerLoadoutId.Value));
                }

                if (death.CharacterLoadoutId != null && !loadoutHash.ContainsKey(death.CharacterLoadoutId.Value))
                {
                    loadoutHash.Add(death.CharacterLoadoutId.Value, new CombatReportClassStats(death.CharacterLoadoutId.Value));
                }
            }

            foreach (var death in vehicleDeaths)
            {
                if (!participantHash.ContainsKey(death.AttackerCharacterId))
                {
                    participantHash.Add(death.AttackerCharacterId, new CombatReportParticipantStats(death.AttackerCharacterId));
                }

                if (death.VehicleId != null && death.VehicleId != 0 && !vehicleHash.ContainsKey(death.VehicleId.Value))
                {
                    vehicleHash.Add(death.VehicleId.Value, new CombatReportVehicleStats(death.VehicleId.Value));
                }
            }
        }

        private async Task LoadCombatStatDetails(Dictionary<string, CombatReportParticipantStats> participantHash,
            Dictionary<string, CombatReportOutfitStats> outfitHash,
            Dictionary<int, CombatReportVehicleStats> vehicleHash,
            Dictionary<int, CombatReportWeaponStats> weaponHash,
            Dictionary<int, CombatReportClassStats> loadoutHash)
        {
            var charactersTask = _characterService.FindCharacters(participantHash.Keys);
            var outfitsTask = _outfitService.FindOutfits(outfitHash.Keys);
            var weaponsTask = _itemService.FindItems(weaponHash.Keys);
            var vehiclesTask = _vehicleService.GetAllVehicles();

            await Task.WhenAll(charactersTask, outfitsTask, weaponsTask, vehiclesTask);

            foreach (var outfit in outfitsTask.Result)
            {
                outfitHash[outfit.Id].Outfit.Name = outfit.Name;
                outfitHash[outfit.Id].Outfit.FactionId = outfit.FactionId ?? 0;
                outfitHash[outfit.Id].Outfit.Alias = outfit.Alias;
            }

            if (outfitHash.ContainsKey(""))
            {
                outfitHash[""].Outfit.Name = "No Outfit";
            }

            foreach (var character in charactersTask.Result)
            {
                var hashCharacter = participantHash[character.Id];

                hashCharacter.Character.Name = character.Name;
                hashCharacter.Character.FactionId = character.FactionId;
                hashCharacter.Character.BattleRank = character.BattleRank;
                hashCharacter.Character.PrestigeLevel = character.PrestigeLevel;
                hashCharacter.Character.WorldId = character.WorldId;

                if (hashCharacter.Outfit != null)
                {
                    hashCharacter.Outfit = outfitHash[hashCharacter.Outfit.Id].Outfit;
                }
            }

            foreach (var weapon in weaponsTask.Result)
            {
                weaponHash[weapon.Id].Item.Name = weapon.Name;
                weaponHash[weapon.Id].Item.FactionId = weapon.FactionId ?? 0;
            }

            foreach (var vehicle in vehiclesTask.Result)
            {
                if (vehicleHash.ContainsKey(vehicle.Id))
                {
                    vehicleHash[vehicle.Id].Vehicle.Name = vehicle.Name;
                    if (vehicle.Factions != null && vehicle.Factions.Count() == 1)
                    {
                        vehicleHash[vehicle.Id].Vehicle.FactionId = vehicle.Factions.First();
                    }
                }
            }

            foreach (var loadoutId in loadoutHash.Keys)
            {
                var profile = await _profileService.GetProfileFromLoadoutIdAsync(loadoutId);
                if (profile == null)
                {
                    continue;
                }

                loadoutHash[loadoutId].Profile.ProfileId = profile.Id;
                loadoutHash[loadoutId].Profile.Name = profile.Name;
                loadoutHash[loadoutId].Profile.ProfileTypeId = profile.ProfileTypeId;
                loadoutHash[loadoutId].Profile.ProfileImageId = profile.ImageId;
                loadoutHash[loadoutId].Profile.FactionId = profile.FactionId;
            }
        }

        private void CalculateCombatStats(IEnumerable<Death> deaths, IEnumerable<VehicleDestroy> vehicleDeaths,
            IEnumerable<FacilityControl> facilityControls,
            Dictionary<string, CombatReportParticipantStats> participantHash,
            Dictionary<string, CombatReportOutfitStats> outfitHash,
            Dictionary<int, CombatReportVehicleStats> vehicleHash,
            Dictionary<int, CombatReportWeaponStats> weaponHash,
            Dictionary<int, CombatReportClassStats> loadoutHash)
        {
            foreach (var death in deaths)
            {
                CombatReportOutfitStats attackerOutfit = null;
                CombatReportOutfitStats victimOutfit = null;
                CombatReportWeaponStats weapon = null;
                CombatReportVehicleStats vehicle = null;
                CombatReportClassStats attackerClass = null;
                CombatReportClassStats victimClass = null;

                if (death.AttackerOutfitId != null)
                {
                    outfitHash.TryGetValue(death.AttackerOutfitId, out attackerOutfit);
                }

                if (death.CharacterOutfitId != null)
                {
                    outfitHash.TryGetValue(death.CharacterOutfitId, out victimOutfit);
                }

                if (death.AttackerWeaponId.HasValue && weaponHash.ContainsKey(death.AttackerWeaponId.Value))
                {
                    weapon = weaponHash[death.AttackerWeaponId.Value];
                }

                if (death.AttackerVehicleId.HasValue && vehicleHash.ContainsKey(death.AttackerVehicleId.Value))
                {
                    vehicle = vehicleHash[death.AttackerVehicleId.Value];
                }

                if (death.AttackerLoadoutId != null)
                {
                    loadoutHash.TryGetValue(death.AttackerLoadoutId.Value, out attackerClass);
                }

                if (death.CharacterLoadoutId != null)
                {
                    loadoutHash.TryGetValue(death.CharacterLoadoutId.Value, out victimClass);
                }

                if (participantHash.TryGetValue(death.CharacterId, out var victim))
                {
                    if (death.AttackerCharacterId == death.CharacterId || death.AttackerCharacterId == "0")
                    {
                        victim.Suicides++;

                        if (victimOutfit != null) { victimOutfit.Suicides++; }
                        if (victimClass != null) { victimClass.Suicides++; }
                    }
                    else if (participantHash.TryGetValue(death.AttackerCharacterId, out var attacker))
                    {
                        if (attacker.Character.FactionId == victim.Character.FactionId)
                        {
                            attacker.Teamkills++;

                            if (attackerOutfit != null) { attackerOutfit.Teamkills++; }
                            if (attackerClass != null) { attackerClass.Teamkills++; }
                            if (weapon != null) { weapon.Teamkills++; }
                            if (vehicle != null) { vehicle.Teamkills++; }
                        }
                        else
                        {
                            attacker.Kills++;

                            if (attackerOutfit != null) { attackerOutfit.Kills++; }
                            if (attackerClass != null) { attackerClass.Kills++; }
                            if (weapon != null) { weapon.Kills++; }
                            if (vehicle != null) { vehicle.Kills++; }

                            if (death.IsHeadshot)
                            {
                                attacker.Headshots++;

                                if (attackerOutfit != null) { attackerOutfit.Headshots++; }
                                if (attackerClass != null) { attackerClass.Headshots++; }
                                if (weapon != null) { weapon.Headshots++; }
                            }
                        }
                    }

                    victim.Deaths++;
                }

                if (victimOutfit != null) { victimOutfit.Deaths++; }
                if (victimClass != null) { victimClass.Deaths++; }
            }

            foreach (var death in vehicleDeaths)
            {
                if (participantHash.TryGetValue(death.AttackerCharacterId, out var attacker))
                {
                    attacker.VehicleKills++;

                    if (attacker.Outfit?.Id != null && outfitHash.ContainsKey(attacker.Outfit.Id))
                    {
                        outfitHash[attacker.Outfit.Id].VehicleKills++;
                    }
                }

                if (death.AttackerLoadoutId != null && loadoutHash.ContainsKey(death.AttackerLoadoutId.Value))
                {
                    loadoutHash[death.AttackerLoadoutId.Value].VehicleKills++;
                }

                if (death.AttackerVehicleId.HasValue && vehicleHash.ContainsKey(death.AttackerVehicleId.Value))
                {
                    vehicleHash[death.AttackerVehicleId.Value].Deaths++;
                }
            }

            var outfitFacilityCaptures = facilityControls.Where(a => a.NewFactionId != a.OldFactionId)
                .GroupBy(a => a.OutfitId ?? "")
                .ToDictionary(a => a.Key, a => a.Count());

            foreach (var outfit in outfitHash)
            {
                outfitHash[outfit.Key].FacilityCaptures = outfitFacilityCaptures.GetValueOrDefault(outfit.Key, 0);
                outfitHash[outfit.Key].ParticipantCount = participantHash.Values.Count(a => a.Outfit?.Id == outfit.Key);
            }
        }

        private void SetCombatStatTopItems(List<Death> deaths,
            Dictionary<string, CombatReportParticipantStats> participantHash,
            Dictionary<int, CombatReportWeaponStats> weaponHash,
            Dictionary<int, CombatReportClassStats> loadoutHash)
        {
            var attackerKillGroups = deaths.Where(a =>
                    !string.IsNullOrEmpty(a.AttackerCharacterId) &&
                    !string.IsNullOrEmpty(a.CharacterId) &&
                    a.AttackerCharacterId != a.CharacterId &&
                    participantHash.ContainsKey(a.AttackerCharacterId))
                .GroupBy(a => a.AttackerCharacterId)
                .ToList();

            foreach (var attackerKills in attackerKillGroups)
            {
                var characterId = attackerKills.Key;

                var topWeaponId = attackerKills.GroupBy(b => b.AttackerWeaponId).OrderByDescending(b => b.Count())
                    .FirstOrDefault()?.Key;

                var topLoadoutId = attackerKills.GroupBy(b => b.AttackerLoadoutId).OrderByDescending(b => b.Count())
                    .FirstOrDefault()?.Key;

                participantHash[characterId].TopWeaponId = topWeaponId;
                participantHash[characterId].TopProfileId = topLoadoutId;

                if (topWeaponId != null && weaponHash.TryGetValue(topWeaponId.Value, out var weapon))
                {
                    participantHash[characterId].TopWeaponName = weapon.Item?.Name;
                }

                if (topLoadoutId != null && loadoutHash.TryGetValue(topLoadoutId.Value, out var loadout))
                {
                    participantHash[characterId].TopProfileId = loadout.Profile?.ProfileId;
                    participantHash[characterId].TopProfileImageId = loadout.Profile?.ProfileImageId;
                    participantHash[characterId].TopProfileName = loadout.Profile?.Name;
                }
            }
        }

        private Task<IEnumerable<Death>> GetCharacterDeaths(int worldId, DateTime startDate, DateTime? endDate, int? zoneId)
        {
            return _worldEventsService.GetDeathEventsByDateAsync(worldId, startDate, endDate, zoneId);
        }

        private Task<IEnumerable<VehicleDestroy>> GetVehicleDeaths(int worldId, DateTime startDate, DateTime? endDate, int? zoneId)
        {
            return _worldEventsService.GetVehicleDeathEventsByDateAsync(worldId, startDate, endDate, zoneId);
        }

        private async Task<IEnumerable<CaptureLogRow>> GetCaptureLog(int worldId, int zoneId, DateTime startDate, DateTime? endDate)
        {
            var facilityControls = await _worldEventsService.GetFacilityControlsByDateAsync(worldId, startDate, endDate, zoneId);

            var facilityIds = facilityControls.Select(c => c.FacilityId).Distinct().ToArray();
            var mapRegions = await _mapService.FindRegions(facilityIds);

            var controlOutfitIds = facilityControls.Select(a => a.OutfitId).Distinct().ToList();
            var outfits = controlOutfitIds.Any() ? await _outfitService.FindOutfits(controlOutfitIds) : Enumerable.Empty<Outfit>();

            return facilityControls.Select(control => {
                var row = new CaptureLogRow
                {
                    FactionVs = control.ZoneControlVs,
                    FactionNc = control.ZoneControlNc,
                    FactionTr = control.ZoneControlTr,
                    FactionNs = control.ZoneControlNs,
                    ZonePopVs = control.ZonePopulationVs,
                    ZonePopNc = control.ZonePopulationNc,
                    ZonePopTr = control.ZonePopulationTr,
                    ZonePopNs = control.ZonePopulationNs,
                    NewFactionId = control.NewFactionId,
                    OldFactionId = control.OldFactionId,
                    Timestamp = control.Timestamp,
                    MapRegion = new CaptureLogRowMapRegion { Id = control.FacilityId }
                };

                var outfit = outfits.FirstOrDefault(a => a.Id == control.OutfitId);
                if (outfit != null)
                {
                    row.Outfit = new CaptureLogRowOutfit {
                        Id = outfit.Id,
                        Alias = outfit.Alias,
                        Name = outfit.Name
                    };
                }

                var mapRegion = mapRegions.FirstOrDefault(a => a.FacilityId == control.FacilityId);
                if (mapRegion != null)
                {
                    row.MapRegion = new CaptureLogRowMapRegion
                    {
                        Id = mapRegion.FacilityId,
                        FacilityName = mapRegion.FacilityName,
                        FacilityType = mapRegion.FacilityType
                    };
                }

                return row;
            });
        }
    }
}
