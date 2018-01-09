﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.Census.JsonConverters;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Websocket.Models;

namespace Voidwell.DaybreakGames.Websocket
{
    public class WebsocketEventHandler : IWebsocketEventHandler
    {
        private readonly IEventRepository _eventRepository;
        private readonly IAlertRepository _alertRepository;
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

        public WebsocketEventHandler(IEventRepository eventRepository, IAlertRepository alertRepository, IWorldMonitor worldMonitor, ICharacterService characterService, IAlertService alertService, ILogger<WebsocketEventHandler> logger)
        {
            _eventRepository = eventRepository;
            _alertRepository = alertRepository;
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
            var jType = message.SelectToken("type");
            if (jType != null && jType.Value<string>() == "serviceStateChanged")
            {
                var detail = message.SelectToken("detail").Value<string>();

                var regServer = @"EventServerEndpoint_(.*)_(.*)";
                Regex r = new Regex(regServer);
                Match m = r.Match(detail);

                var worldName = m.Groups[1].Value;
                var worldId = m.Groups[2].Value;
                var isWorldOnline = message.SelectToken("online").Value<bool>();

                await _worldMonitor.SetWorldState(worldId, worldName, isWorldOnline);
                return;
            }

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
            return _eventRepository.AddAsync(dataModel);
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
            return _eventRepository.AddAsync(dataModel);
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
            return _eventRepository.AddAsync(dataModel);
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
            return _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("Death", typeof(Death))]
        private async Task Process(Death payload)
        {
            List<Task> outfitWork = new List<Task>();
            Task<DbOutfitMember> AttackerOutfitTask = null;
            Task<DbOutfitMember> VictimOutfitTask = null;

            if (payload.AttackerCharacterId != null && payload.AttackerCharacterId.Length > 18)
            {
                await _characterService.GetCharacter(payload.AttackerCharacterId);
                AttackerOutfitTask = _characterService.GetCharactersOutfit(payload.AttackerCharacterId);
                outfitWork.Add(AttackerOutfitTask);
            }

            if (payload.CharacterId != null && payload.CharacterId.Length > 18)
            {
                await _characterService.GetCharacter(payload.CharacterId);
                VictimOutfitTask = _characterService.GetCharactersOutfit(payload.CharacterId);
                outfitWork.Add(VictimOutfitTask);
            }

            await Task.WhenAll(outfitWork);

            var dataModel = new DbEventDeath
            {
                AttackerCharacterId = payload.AttackerCharacterId,
                AttackerFireModeId = payload.AttackerFireModeId,
                AttackerLoadoutId = payload.AttackerLoadoutId,
                AttackerVehicleId = payload.AttackerVehicleId,
                AttackerWeaponId = payload.AttackerWeaponId,
                AttackerOutfitId = AttackerOutfitTask?.Result?.OutfitId,
                CharacterId = payload.CharacterId,
                CharacterLoadoutId = payload.CharacterLoadoutId,
                CharacterOutfitId = VictimOutfitTask?.Result?.OutfitId,
                IsHeadshot = payload.IsHeadshot,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId
            };

            await _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("FacilityControl", typeof(FacilityControl))]
        private async Task Process(FacilityControl payload)
        {
            var mapUpdate = _worldMonitor.UpdateFacilityControl(payload);
            var territory = mapUpdate?.Territory.ToArray();

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
                ZoneControlVs = territory != null ? territory[1] * 100 : 0,
                ZoneControlNc = territory != null ? territory[2] * 100 : 0,
                ZoneControlTr = territory != null ? territory[3] * 100 : 0
            };

            await _eventRepository.AddAsync(dataModel);

            if (dataModel.NewFactionId != dataModel.OldFactionId)
            {
                var alert = await _alertRepository.GetActiveAlert(dataModel.WorldId, dataModel.ZoneId);

                if (alert == null)
                {
                    return;
                }

                alert.LastFactionVs = dataModel.ZoneControlVs;
                alert.LastFactionNc = dataModel.ZoneControlNc;
                alert.LastFactionTr = dataModel.ZoneControlTr;

                await _alertRepository.UpdateAsync(alert);
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
            return _eventRepository.AddAsync(dataModel);
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
            await _eventRepository.AddAsync(dataModel);

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
            return _eventRepository.AddAsync(dataModel);
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
            return _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("PlayerLogin", typeof(PlayerLogin))]
        private async Task Process(PlayerLogin payload)
        {
            await _worldMonitor.SetPlayerOnlineState(payload.CharacterId, payload.Timestamp, true);

            var dataModel = new DbEventPlayerLogin
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };
            await _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("PlayerLogout", typeof(PlayerLogout))]
        private async Task Process(PlayerLogout payload)
        {
            await _worldMonitor.SetPlayerOnlineState(payload.CharacterId, payload.Timestamp, false);

            var dataModel = new DbEventPlayerLogout
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };
            await _eventRepository.AddAsync(dataModel);
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
            return _eventRepository.AddAsync(dataModel);
        }
    }
}
