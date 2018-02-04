using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CombatReportService : ICombatReportService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICharacterService _characterService;
        private readonly IOutfitService _outfitService;
        private readonly IItemService _itemService;
        private readonly IVehicleService _vehicleService;
        private readonly IMapService _mapService;

        public CombatReportService(IEventRepository eventRepository, ICharacterService characterService,
            IOutfitService outfitService, IItemService itemService, IVehicleService vehicleService, IMapService mapService)
        {
            _eventRepository = eventRepository;
            _characterService = characterService;
            _outfitService = outfitService;
            _itemService = itemService;
            _vehicleService = vehicleService;
            _mapService = mapService;
        }

        public async Task<CombatReport> GetCombatReport(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            var combatStatsTask = GetCombatStats(worldId, zoneId, startDate, endDate);
            var captureLogTask = GetCaptureLog(worldId, zoneId, startDate, endDate);

            await Task.WhenAll(combatStatsTask, captureLogTask);

            var combatReport = new CombatReport
            {
                Stats = combatStatsTask.Result,
                CaptureLog = captureLogTask.Result
            };

            return combatReport;
        }

        private async Task<CombatReportStats> GetCombatStats(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            var characterDeathsTask = GetCharacterDeaths(worldId, zoneId, startDate, endDate);
            var vehicleDeathsTask = GetVehicleDeaths(worldId, zoneId, startDate, endDate);

            await Task.WhenAll(characterDeathsTask, vehicleDeathsTask);

            var deaths = characterDeathsTask.Result;
            var vehicleDeaths = vehicleDeathsTask.Result;

            var participantHash = new Dictionary<string, CombatReportParticipantStats>();
            var outfitHash = new Dictionary<string, CombatReportOutfitStats>();
            var vehicleHash = new Dictionary<string, CombatReportVehicleStats>();
            var weaponHash = new Dictionary<string, CombatReportWeaponStats>();

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

                if (death.AttackerWeaponId != "0" && !weaponHash.ContainsKey(death.AttackerWeaponId))
                {
                    weaponHash.Add(death.AttackerWeaponId, new CombatReportWeaponStats(death.AttackerWeaponId));
                }

                if (death.AttackerVehicleId != null && death.AttackerVehicleId != "0" && !vehicleHash.ContainsKey(death.AttackerVehicleId))
                {
                    vehicleHash.Add(death.AttackerVehicleId, new CombatReportVehicleStats(death.AttackerVehicleId));
                }
            }

            foreach (var death in vehicleDeaths)
            {
                if (!participantHash.ContainsKey(death.AttackerCharacterId))
                {
                    participantHash.Add(death.AttackerCharacterId, new CombatReportParticipantStats(death.AttackerCharacterId));
                }

                if (death.VehicleId != null && death.VehicleId != "0" && !vehicleHash.ContainsKey(death.VehicleId))
                {
                    vehicleHash.Add(death.VehicleId, new CombatReportVehicleStats(death.VehicleId));
                }
            }

            var charactersTask = _characterService.FindCharacters(participantHash.Keys);
            var outfitsTask = _outfitService.FindOutfits(outfitHash.Keys);
            var weaponsTask = _itemService.FindItems(weaponHash.Keys);
            var vehiclesTask = _vehicleService.GetAllVehicles();

            await Task.WhenAll(charactersTask, outfitsTask, weaponsTask, vehiclesTask);

            foreach (var outfit in outfitsTask.Result)
            {
                outfitHash[outfit.Id].Outfit.Name = outfit.Name;
                outfitHash[outfit.Id].Outfit.FactionId = outfit.FactionId;
                outfitHash[outfit.Id].Outfit.Alias = outfit.Alias;
            }

            foreach (var character in charactersTask.Result)
            {
                var hashCharacter = participantHash[character.Id];

                hashCharacter.Character.Name = character.Name;
                hashCharacter.Character.FactionId = character.FactionId;

                if (hashCharacter.Outfit != null)
                {
                    hashCharacter.Outfit = outfitHash[hashCharacter.Outfit.Id].Outfit;
                }
            }

            foreach (var weapon in weaponsTask.Result)
            {
                weaponHash[weapon.Id].Item.Name = weapon.Name;
                weaponHash[weapon.Id].Item.FactionId = weapon.FactionId;
            }

            foreach(var vehicle in vehiclesTask.Result)
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

            foreach(var death in deaths)
            {
                participantHash.TryGetValue(death.AttackerCharacterId, out var attacker);
                participantHash.TryGetValue(death.CharacterId, out var victim);
                outfitHash.TryGetValue(death.AttackerOutfitId, out var attackerOutfit);
                outfitHash.TryGetValue(death.CharacterOutfitId, out var victimOutfit);
                weaponHash.TryGetValue(death.AttackerWeaponId, out var weapon);
                vehicleHash.TryGetValue(death.AttackerVehicleId, out var vehicle);

                if (attacker.Character.Id == victim.Character.Id || attacker.Character.Id == "0")
                {
                    victim.Suicides++;

                    if (victimOutfit != null) { victimOutfit.Suicides++; }
                }
                else
                {
                    if (attacker.Character.FactionId == victim.Character.FactionId)
                    {
                        attacker.Teamkills++;

                        if (attackerOutfit != null) { attackerOutfit.Teamkills++; }
                        if (weapon != null) { weapon.Teamkills++; }
                        if (vehicle != null) { vehicle.Teamkills++; }
                    }
                    else
                    {
                        attacker.Kills++;

                        if (attackerOutfit != null) { attackerOutfit.Kills++; }
                        if (attackerOutfit != null) { attackerOutfit.VehicleKills++; }
                        if (weapon != null) { weapon.Kills++; }
                        if (vehicle != null) { vehicle.Kills++; }

                        if (death.IsHeadshot)
                        {
                            attacker.Headshots++;

                            if (attackerOutfit != null) { attackerOutfit.Headshots++; }
                            if (weapon != null) { weapon.Headshots++; }
                        }
                    }
                }

                victim.Deaths++;

                if (victimOutfit != null) { victimOutfit.Deaths++; }
            }

            foreach(var death in vehicleDeaths)
            {
                participantHash.TryGetValue(death.AttackerCharacterId, out var attacker);
                vehicleHash.TryGetValue(death.AttackerVehicleId, out var vehicle);

                if (attacker != null) { attacker.VehicleKills++; }
                if (vehicle != null) { vehicle.Deaths++; }
            }

            if (outfitHash.ContainsKey(""))
            {
                outfitHash[""].Outfit.Name = "No Outfit";
            }

            return new CombatReportStats
            {
                Participants = participantHash.Values.ToArray(),
                Outfits = outfitHash.Values.ToArray(),
                Weapons = weaponHash.Values.ToArray(),
                Vehicles = vehicleHash.Values.ToArray()
            };
        }

        private Task<IEnumerable<EventDeath>> GetCharacterDeaths(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            return _eventRepository.GetDeathEventsByDateAsync(worldId, zoneId, startDate, endDate);
        }

        private Task<IEnumerable<EventVehicleDestroy>> GetVehicleDeaths(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            return _eventRepository.GetVehicleDeathEventsByDateAsync(worldId, zoneId, startDate, endDate);
        }

        private async Task<IEnumerable<CaptureLogRow>> GetCaptureLog(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            var facilityControls = await _eventRepository.GetFacilityControlsByDateAsync(worldId, zoneId, startDate, endDate);

            var facilityIds = facilityControls.Select(c => c.FacilityId).Distinct().ToArray();
            var mapRegions = await _mapService.FindRegions(facilityIds);

            var controlOutfitIds = facilityControls.Select(a => a.OutfitId);
            var outfits = controlOutfitIds.Any() ? await _outfitService.FindOutfits(controlOutfitIds) : Enumerable.Empty<Outfit>();

            return facilityControls.Select(control => {
                var row = new CaptureLogRow
                {
                    FactionVs = control.ZoneControlVs,
                    FactionNc = control.ZoneControlNc,
                    FactionTr = control.ZoneControlTr,
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
