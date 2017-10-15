using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.Census.JsonConverters;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Websocket
{
    public class WebsocketEventHandler : IWebsocketEventHandler, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private readonly IWorldMonitor _worldMonitor;
        private readonly ICharacterService _characterService;
        private readonly IAlertService _alertService;
        private readonly ILogger<WebsocketEventHandler> _logger;
        private Dictionary<string, MethodInfo> _processMethods;

        private Dictionary<string, string> _metagameZones = new Dictionary<string, string> { {"1", "2"}, {"2", "8"}, {"3", "6"}, {"4", "4"} };
        private enum METAGAME_EVENT_STATE
        {
            STARTED = 135,
            RESTARTED = 136,
            CANCELED = 137,
            ENDED = 138,
            XPCHANGE = 139
        };
        private JsonSerializer _payloadDeserializer;

        public WebsocketEventHandler(PS2DbContext ps2DbContext, IWorldMonitor worldMonitor, ICharacterService characterService, IAlertService alertService, ILogger<WebsocketEventHandler> logger)
        {
            _ps2DbContext = ps2DbContext;
            _worldMonitor = worldMonitor;
            _characterService = characterService;
            _alertService = alertService;
            _logger = logger;

            _processMethods = GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<CensusEventHandlerAttribute>() != null)
                .ToDictionary(m => m.GetCustomAttribute<CensusEventHandlerAttribute>().EventName);

            var deserializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new UnderscorePropertyNamesContractResolver(),
                Converters = new JsonConverter[]
                {
                    new BooleanJsonConverter(),
                    new DateTimeJsonConverter()
                }
            };
            _payloadDeserializer = JsonSerializer.Create(deserializerSettings);
        }

        public async Task Process(JToken message)
        {
            var jPayload = message.SelectToken("payload");

            var payload = jPayload?.ToObject<PayloadBase>(_payloadDeserializer);
            var eventName = payload?.EventName;

            if (eventName == null)
            {
                return;
            }

            _logger.LogDebug("Payload received for event: {0}.", eventName);

            if (!_processMethods.ContainsKey(eventName))
            {
                _logger.LogWarning("No process method found for event: {0}", eventName);
                return;
            }

            if (payload.ZoneId != null)
            {
                int iZoneId = 0;
                if (!int.TryParse(payload.ZoneId, out iZoneId) || iZoneId > 1000)
                {
                    return;
                }
            }

            try
            {
                var inputType = _processMethods[eventName].GetCustomAttribute<CensusEventHandlerAttribute>().PayloadType;
                var inputParam = jPayload.ToObject(inputType, _payloadDeserializer);

                await (Task)_processMethods[eventName].Invoke(this, new[] { inputParam });
            }
            catch(Exception ex)
            {
                _logger.LogError(75642, ex, "Failed to process websocket event: {0}.", eventName);
            }
        }

        [CensusEventHandler("AchievementEarned", typeof(AchievementEarned))]
        private Task Process(AchievementEarned payload)
        {
            var dataModel = new DbEventAchievementEarned
            {
                AchievementId = payload.AchievementId,
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };
            _ps2DbContext.AchievementEarnedEvents.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        [CensusEventHandler("BattleRankUp", typeof(BattlerankUp))]
        private Task Process(BattlerankUp payload)
        {
            var dataModel = new DbEventBattlerankUp
            {
                BattleRank = payload.BattleRank,
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };
            _ps2DbContext.BattleRankUpEvents.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        [CensusEventHandler("ContinentLock", typeof(ContinentLock))]
        private Task Process(ContinentLock payload)
        {
            var dataModel = new DbEventContinentLock
            {
                TriggeringFaction = payload.TriggeringFaction,
                MetagameEventId = payload.MetagameEventId,
                PopulationVs = payload.VsPopulation,
                PopulationNc = payload.NcPopulation,
                PopulationTr = payload.TrPopulation,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };
            _ps2DbContext.ContinentLockEvents.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        [CensusEventHandler("ContinentUnlock", typeof(ContinentUnlock))]
        private Task Process(ContinentUnlock payload)
        {
            var dataModel = new DbEventContinentUnlock
            {
                TriggeringFaction = payload.TriggeringFaction,
                MetagameEventId = payload.MetagameEventId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };
            _ps2DbContext.ContinentUnlockEvents.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        [CensusEventHandler("Death", typeof(Death))]
        private async Task Process(Death payload)
        {
            List<Task> characterWork = new List<Task>();
            List<Task> outfitWork = new List<Task>();
            Task<DbOutfitMember> AttackerOutfitTask = null;
            Task<DbOutfitMember> VictimOutfitTask = null;
            if (payload.AttackerCharacterId != null && payload.AttackerCharacterId.Length > 18)
            {
                characterWork.Add(_characterService.GetCharacter(payload.AttackerCharacterId));
                AttackerOutfitTask = _characterService.GetCharactersOutfit(payload.AttackerCharacterId);
                outfitWork.Add(AttackerOutfitTask);
            }

            if (payload.CharacterId != null && payload.CharacterId.Length > 18)
            {
                characterWork.Add(_characterService.GetCharacter(payload.CharacterId));
                VictimOutfitTask = _characterService.GetCharactersOutfit(payload.CharacterId);
                outfitWork.Add(VictimOutfitTask);
            }

            await Task.WhenAll(characterWork);
            await Task.WhenAll(outfitWork);

            var dataModel = new DbEventDeath
            {
                AttackerCharacterId = payload.AttackerCharacterId,
                AttackerFireModeId = payload.AttackerFireModeId,
                AttackerLoadoutId = payload.AttackerLoadoutId,
                AttackerVehicleId = payload.AttackerVehicleId,
                AttackerWeaponId = payload.AttackerWeaponId,
                AttackerOutfitId = AttackerOutfitTask?.Result.OutfitId,
                CharacterId = payload.CharacterId,
                CharacterLoadoutId = payload.CharacterLoadoutId,
                CharacterOutfitId = VictimOutfitTask?.Result.OutfitId,
                IsHeadshot = payload.IsHeadshot,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };
            _ps2DbContext.EventDeaths.Add(dataModel);
            await _ps2DbContext.SaveChangesAsync();
        }

        [CensusEventHandler("FacilityControl", typeof(FacilityControl))]
        private async Task Process(FacilityControl payload)
        {
            var mapUpdate = _worldMonitor.UpdateFacilityControl(payload);
            var territory = mapUpdate.Territory.ToArray();

            var dataModel = new DbEventFacilityControl
            {
                FacilityId = payload.FacilityId,
                NewFactionId = payload.NewFactionId,
                OldFactionId = payload.OldFactionId,
                DurationHeld = payload.DurationHeld,
                OutfitId = payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId,
                ZoneControlVs = territory[1] * 100,
                ZoneControlNc = territory[2] * 100,
                ZoneControlTr = territory[3] * 100
            };

            _ps2DbContext.EventFacilityControls.Add(dataModel);
            await _ps2DbContext.SaveChangesAsync();

            if (dataModel.NewFactionId != dataModel.OldFactionId)
            {
                var alert = await _ps2DbContext.Alerts
                    .AsTracking()
                    .SingleOrDefaultAsync(a => a.WorldId == dataModel.WorldId && a.ZoneId == dataModel.ZoneId && a.EndDate == null);

                if (alert == null)
                {
                    return;
                }

                alert.LastFactionVs = dataModel.ZoneControlVs;
                alert.LastFactionNc = dataModel.ZoneControlNc;
                alert.LastFactionTr = dataModel.ZoneControlTr;

                _ps2DbContext.Alerts.Update(alert);
                await _ps2DbContext.SaveChangesAsync();
            }
        }

        [CensusEventHandler("GainExperience", typeof(GainExperience))]
        private Task Process(GainExperience payload)
        {
            var dataModel = new DbEventGainExperience
            {
                ExperienceId = payload.ExperienceId,
                CharacterId = payload.CharacterId,
                Amount = payload.Amount,
                LoadoutId = payload.LoadoutId,
                OtherId = payload.OtherId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };
            _ps2DbContext.GainExperienceEvents.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        [CensusEventHandler("MetagameEvent", typeof(MetagameEvent))]
        private async Task Process(MetagameEvent payload)
        {
            payload.ZoneId = payload.ZoneId ?? (_metagameZones.TryGetValue(payload.MetagameEventId, out string metagameZone) ? metagameZone : null);

            var dataModel = new DbEventMetagameEvent
            {
                InstanceId = payload.InstanceId,
                MetagameEventId = payload.MetagameEventId,
                MetagameEventState = payload.MetagameEventState,
                ZoneControlVs = payload.FactionVs,
                ZoneControlNc = payload.FactionNc,
                ZoneControlTr = payload.FactionTr,
                ExperienceBonus = payload.ExperienceBonus,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };
            _ps2DbContext.MetagameEventEvents.Add(dataModel);
            await _ps2DbContext.SaveChangesAsync();

            var eventState = (METAGAME_EVENT_STATE)Enum.Parse(typeof(METAGAME_EVENT_STATE), dataModel.MetagameEventState);
            if (eventState == METAGAME_EVENT_STATE.STARTED || eventState == METAGAME_EVENT_STATE.RESTARTED)
            {
                await _alertService.CreateAlert(payload);
            }
            else if (eventState == METAGAME_EVENT_STATE.ENDED || eventState == METAGAME_EVENT_STATE.CANCELED)
            {
                await _alertService.UpdateAlert(payload);
            }
        }

        [CensusEventHandler("PlayerFacilityCapture", typeof(PlayerFacilityCapture))]
        private Task Proces(PlayerFacilityCapture payload)
        {
            var dataModel = new DbEventPlayerFacilityCapture
            {
                FacilityId = payload.FacilityId,
                CharacterId = payload.CharacterId,
                OutfitId = payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };
            _ps2DbContext.PlayerFacilityCaptureEvents.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        [CensusEventHandler("PlayerFacilityDefend", typeof(PlayerFacilityDefend))]
        private Task Process(PlayerFacilityDefend payload)
        {
            var dataModel = new DbEventPlayerFacilityDefend
            {
                FacilityId = payload.FacilityId,
                CharacterId = payload.CharacterId,
                OutfitId = payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };
            _ps2DbContext.PlayerFacilityDefendEvents.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        [CensusEventHandler("PlayerLogin", typeof(PlayerLogin))]
        private Task Process(PlayerLogin payload)
        {
            _worldMonitor.SetPlayerOnlineState(payload.CharacterId, payload.Timestamp, true);

            var dataModel = new DbEventPlayerLogin
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };
            _ps2DbContext.PlayerLoginEvents.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        [CensusEventHandler("PlayerLogout", typeof(PlayerLogout))]
        private Task Process(PlayerLogout payload)
        {
            _worldMonitor.SetPlayerOnlineState(payload.CharacterId, payload.Timestamp, false);

            var dataModel = new DbEventPlayerLogout
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };
            _ps2DbContext.PlayerLogoutEvents.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        [CensusEventHandler("VehicleDestroy", typeof(VehicleDestroy))]
        private Task Process(VehicleDestroy payload)
        {
            var dataModel = new DbEventVehicleDestroy
            {
                AttackerCharacterId = payload.AttackerCharacterId,
                AttackerLoadoutId = payload.AttackerLoadoutId,
                AttackerVehicleId = payload.AttackerVehicleId,
                AttackerWeaponId = payload.AttackerWeaponId,
                CharacterId = payload.CharacterId,
                VehicleId = payload.VehicleId,
                FactionId = payload.FactionId,
                FacilityId = payload.FacilityId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };
            _ps2DbContext.EventVehicleDestroys.Add(dataModel);
            return _ps2DbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
