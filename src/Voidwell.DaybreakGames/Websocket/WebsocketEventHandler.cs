using Microsoft.EntityFrameworkCore;
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

        private Dictionary<int, int> _metagameZones = new Dictionary<int, int> {
            { 1, 2},
            { 2, 8},
            { 3, 6},
            { 4, 4},
            { 123, 2 },
            { 124, 2 },
            { 125, 2 },
            { 126, 8 },
            { 127, 8 },
            { 128, 8 },
            { 129, 6 },
            { 130, 6 },
            { 131, 6 },
            { 132, 4 },
            { 133, 4 },
            { 134, 4 }
        };
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

                if (int.TryParse(m.Groups[2].Value, out var worldId))
                {
                    var isWorldOnline = message.SelectToken("online").Value<bool>();

                    await _worldMonitor.SetWorldState(worldId, worldName, isWorldOnline);
                };

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

            if (payload.ZoneId.HasValue && payload.ZoneId.Value > 1000)
            {
                return;
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

        [CensusEventHandler("AchievementEarned", typeof(Models.AchievementEarned))]
        private Task Process(Models.AchievementEarned payload)
        {
            var dataModel = new Data.Models.Planetside.Events.AchievementEarned
            {
                AchievementId = payload.AchievementId,
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };
            return _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("BattleRankUp", typeof(BattlerankUp))]
        private Task Process(BattlerankUp payload)
        {
            var dataModel = new Data.Models.Planetside.Events.BattlerankUp
            {
                BattleRank = payload.BattleRank,
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };
            return _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("ContinentLock", typeof(Models.ContinentLock))]
        private Task Process(Models.ContinentLock payload)
        {
            var dataModel = new Data.Models.Planetside.Events.ContinentLock
            {
                TriggeringFaction = payload.TriggeringFaction,
                MetagameEventId = payload.MetagameEventId,
                PopulationVs = payload.VsPopulation,
                PopulationNc = payload.NcPopulation,
                PopulationTr = payload.TrPopulation,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };
            return _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("ContinentUnlock", typeof(ContinentUnlock))]
        private Task Process(ContinentUnlock payload)
        {
            var dataModel = new Data.Models.Planetside.Events.ContinentUnlock
            {
                TriggeringFaction = payload.TriggeringFaction,
                MetagameEventId = payload.MetagameEventId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };
            return _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("Death", typeof(Models.Death))]
        private async Task Process(Models.Death payload)
        {
            List<Task> outfitWork = new List<Task>();
            Task<OutfitMember> AttackerOutfitTask = null;
            Task<OutfitMember> VictimOutfitTask = null;

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

            var dataModel = new Data.Models.Planetside.Events.Death
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
                ZoneId = payload.ZoneId.Value
            };

            await _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("FacilityControl", typeof(FacilityControl))]
        private async Task Process(FacilityControl payload)
        {
            var mapUpdate = _worldMonitor.UpdateFacilityControl(payload);
            var territory = mapUpdate?.Territory.ToArray();

            var dataModel = new Data.Models.Planetside.Events.FacilityControl
            {
                FacilityId = payload.FacilityId,
                NewFactionId = payload.NewFactionId,
                OldFactionId = payload.OldFactionId,
                DurationHeld = payload.DurationHeld,
                OutfitId = payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value,
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

        [CensusEventHandler("GainExperience", typeof(Models.GainExperience))]
        private Task Process(Models.GainExperience payload)
        {
            var dataModel = new Data.Models.Planetside.Events.GainExperience
            {
                ExperienceId = payload.ExperienceId,
                CharacterId = payload.CharacterId,
                Amount = payload.Amount,
                LoadoutId = payload.LoadoutId,
                OtherId = payload.OtherId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };
            return _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("MetagameEvent", typeof(MetagameEvent))]
        private async Task Process(MetagameEvent payload)
        {
            payload.ZoneId = payload.ZoneId ?? (_metagameZones.TryGetValue(payload.MetagameEventId, out int metagameZone) ? (int?)metagameZone : null);

            var dataModel = new Data.Models.Planetside.Events.MetagameEvent
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

            var eventState = Enum.Parse<METAGAME_EVENT_STATE>(dataModel.MetagameEventState);
            if (eventState == METAGAME_EVENT_STATE.STARTED || eventState == METAGAME_EVENT_STATE.RESTARTED)
            {
                await _alertService.CreateAlert(payload);
            }
            else if (eventState == METAGAME_EVENT_STATE.ENDED || eventState == METAGAME_EVENT_STATE.CANCELED)
            {
                await _alertService.UpdateAlert(payload);
            }
        }

        [CensusEventHandler("PlayerFacilityCapture", typeof(Models.PlayerFacilityCapture))]
        private Task Proces(Models.PlayerFacilityCapture payload)
        {
            var dataModel = new Data.Models.Planetside.Events.PlayerFacilityCapture
            {
                FacilityId = payload.FacilityId,
                CharacterId = payload.CharacterId,
                OutfitId = payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };
            return _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("PlayerFacilityDefend", typeof(PlayerFacilityDefend))]
        private Task Process(PlayerFacilityDefend payload)
        {
            var dataModel = new Data.Models.Planetside.Events.PlayerFacilityDefend
            {
                FacilityId = payload.FacilityId,
                CharacterId = payload.CharacterId,
                OutfitId = payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };
            return _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("PlayerLogin", typeof(Models.PlayerLogin))]
        private async Task Process(Models.PlayerLogin payload)
        {
            await _worldMonitor.SetPlayerOnlineState(payload.CharacterId, payload.Timestamp, true);

            var dataModel = new Data.Models.Planetside.Events.PlayerLogin
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };
            await _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("PlayerLogout", typeof(Models.PlayerLogout))]
        private async Task Process(Models.PlayerLogout payload)
        {
            await _worldMonitor.SetPlayerOnlineState(payload.CharacterId, payload.Timestamp, false);

            var dataModel = new Data.Models.Planetside.Events.PlayerLogout
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };
            await _eventRepository.AddAsync(dataModel);
        }

        [CensusEventHandler("VehicleDestroy", typeof(Models.VehicleDestroy))]
        private Task Process(Models.VehicleDestroy payload)
        {
            var dataModel = new Data.Models.Planetside.Events.VehicleDestroy
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
                ZoneId = payload.ZoneId.Value
            };
            return _eventRepository.AddAsync(dataModel);
        }
    }
}
