using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CombatReportService : ICombatReportService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly ICharacterService _characterService;
        private readonly IOutfitService _outfitService;
        private readonly IItemService _itemService;
        private readonly IVehicleService _vehicleService;
        private readonly IMapService _mapService;

        public CombatReportService(PS2DbContext ps2DbContext, ICharacterService characterService,
            IOutfitService outfitService, IItemService itemService, IVehicleService vehicleService, IMapService mapService)
        {
            _ps2DbContext = ps2DbContext;
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

            return new CombatReport
            {
                Stats = combatStatsTask.Result,
                CaptureLog = captureLogTask.Result
            };
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

                if (death.AttackerVehicleId != null && !vehicleHash.ContainsKey(death.AttackerVehicleId))
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

                if (death.VehicleId != null && death.VehicleId != "0")
                {
                    if (!vehicleHash.ContainsKey(death.VehicleId))
                    {
                        vehicleHash.Add(death.VehicleId, new CombatReportVehicleStats(death.VehicleId));
                    }
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
                vehicleHash[vehicle.Id].Vehicle.Name = vehicle.Name;
                if (vehicle.Faction != null && vehicle.Faction.Count() == 1)
                {
                    vehicleHash[vehicle.Id].Vehicle.FactionId = vehicle.Faction.First().FactionId;
                }
            }

            foreach(var death in deaths)
            {
                var attacker = participantHash[death.AttackerCharacterId];
                var victim = participantHash[death.CharacterId];
                var attackerOutfit = outfitHash[death.AttackerOutfitId];
                var victimOutfit = outfitHash[death.CharacterOutfitId];
                var weapon = weaponHash[death.AttackerWeaponId];
                var vehicle = vehicleHash[death.AttackerVehicleId];

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
                var attacker = participantHash[death.AttackerCharacterId];
                var vehicle = vehicleHash[death.AttackerVehicleId];

                if (attacker != null) { attacker.VehicleKills++; }
                if (vehicle != null) { vehicle.Deaths++; }
            }

            return new CombatReportStats
            {
                Participants = participantHash.Values.ToArray(),
                Outfits = outfitHash.Values.ToArray(),
                Weapons = weaponHash.Values.ToArray(),
                Vehicles = vehicleHash.Values.ToArray()
            };
        }

        private async Task<IEnumerable<DbEventDeath>> GetCharacterDeaths(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            return await _ps2DbContext.EventDeaths.Where(e => e.WorldId == worldId && e.ZoneId == zoneId && e.Timestamp < endDate && e.Timestamp > startDate)
                .ToListAsync();
        }

        private async Task<IEnumerable<DbEventVehicleDestroy>> GetVehicleDeaths(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            return await _ps2DbContext.EventVehicleDestroys.Where(e => e.WorldId == worldId && e.ZoneId == zoneId && e.Timestamp < endDate && e.Timestamp > startDate)
                .ToListAsync();
        }

        private async Task<IEnumerable<CaptureLogRow>> GetCaptureLog(string worldId, string zoneId, DateTime startDate, DateTime endDate)
        {
            var facilityControls = await _ps2DbContext.EventFacilityControls.Where(e => e.WorldId == worldId && e.ZoneId == zoneId && e.Timestamp < endDate && e.Timestamp > startDate)
                .ToListAsync();

            var facilityIds = facilityControls.Select(c => c.FacilityId).Distinct().ToArray();
            var mapRegions = await _mapService.FindRegions(facilityIds);

            var logRows = facilityControls.Select(c => new CaptureLogRow
            {
                FactionVs = c.ZoneControlVs,
                FactionNc = c.ZoneControlNc,
                FactionTr = c.ZoneControlTr,
                NewFactionId = c.NewFactionId,
                OldFactionId = c.OldFactionId,
                OutfitId = c.OutfitId,
                Timestamp = c.Timestamp,
                MapRegion = new CaptureLogRowMapRegion { Id = c.FacilityId }
            });

            foreach(var mapRegion in mapRegions)
            {
                var rows = logRows.Where(r => r.MapRegion.Id == mapRegion.FacilityId);
                foreach (var row in rows)
                {
                    row.MapRegion.FacilityName = mapRegion.FacilityName;
                    row.MapRegion.FacilityType = mapRegion.FacilityType;
                }
            }

            return logRows;
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
