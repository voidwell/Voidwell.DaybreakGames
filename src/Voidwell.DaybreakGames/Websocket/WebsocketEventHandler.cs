using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.Census.JsonConverters;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Models;
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

        private SemaphoreSlim _continentUnlockSemaphore;
        private SemaphoreSlim _playerFacilityCaptureSemaphore;
        private SemaphoreSlim _playerFacilityDefendSemaphore;
        private SemaphoreSlim _facilityControlSemaphore;

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

            _continentUnlockSemaphore = new SemaphoreSlim(1);
            _playerFacilityCaptureSemaphore = new SemaphoreSlim(5);
            _playerFacilityDefendSemaphore = new SemaphoreSlim(5);
            _facilityControlSemaphore = new SemaphoreSlim(3);
        }

        public async Task Process(JToken message)
        {
            if (message.Value<string>("type") == "serviceStateChanged")
            {
                try
                {
                    await ProcessServiceStateChanged(message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(75843, ex, "Failed to process service state change.");
                }

                return;
            }

            await ProcessServiceEvent(message);
        }

        private async Task ProcessServiceStateChanged(JToken message)
        {
            var detail = message.Value<string>("detail");

            var regServer = @"EventServerEndpoint_(.*)_(.*)";
            Regex r = new Regex(regServer);
            Match m = r.Match(detail);

            var worldName = m.Groups[1].Value;

            if (int.TryParse(m.Groups[2].Value, out var worldId))
            {
                var isWorldOnline = message.Value<bool>("online");

                await _worldMonitor.SetWorldState(worldId, worldName, isWorldOnline);
            };
        }

        private async Task ProcessServiceEvent(JToken message)
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
            catch (Exception ex)
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
        private async Task Process(BattlerankUp payload)
        {
            var dataModel = new Data.Models.Planetside.Events.BattlerankUp
            {
                BattleRank = payload.BattleRank,
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };
            await _eventRepository.AddAsync(dataModel);
            _worldMonitor.SetPlayerLastSeen(dataModel.WorldId, dataModel.CharacterId, dataModel.Timestamp, dataModel.ZoneId);
        }

        [CensusEventHandler("ContinentLock", typeof(Models.ContinentLock))]
        private Task Process(Models.ContinentLock payload)
        {
            var model = new Data.Models.Planetside.Events.ContinentLock
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

            _worldMonitor.UpdateZoneLock(model.WorldId, model.ZoneId, new ZoneLockState(model.Timestamp, model.MetagameEventId, model.TriggeringFaction));

            return _eventRepository.AddAsync(model);
        }

        [CensusEventHandler("ContinentUnlock", typeof(ContinentUnlock))]
        private async Task Process(ContinentUnlock payload)
        {
            var model = new Data.Models.Planetside.Events.ContinentUnlock
            {
                TriggeringFaction = payload.TriggeringFaction,
                MetagameEventId = payload.MetagameEventId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };

            _worldMonitor.UpdateZoneLock(model.WorldId, model.ZoneId);

            await _continentUnlockSemaphore.WaitAsync();

            try
            {
                await _eventRepository.AddAsync(model);
            }
            finally
            {
                _continentUnlockSemaphore.Release();
            }
        }

        [CensusEventHandler("Death", typeof(Models.Death))]
        private async Task Process(Models.Death payload)
        {
            List<Task> outfitWork = new List<Task>();
            Task<OutfitMember> AttackerOutfitTask = null;
            Task<OutfitMember> VictimOutfitTask = null;

            if (payload.AttackerCharacterId != null && payload.AttackerCharacterId.Length > 18)
            {
                AttackerOutfitTask = _characterService.GetCharactersOutfit(payload.AttackerCharacterId);
                outfitWork.Add(AttackerOutfitTask);
            }

            if (payload.CharacterId != null && payload.CharacterId.Length > 18)
            {
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
            _worldMonitor.SetPlayerLastSeen(dataModel.WorldId, dataModel.AttackerCharacterId, dataModel.Timestamp, dataModel.ZoneId);
            _worldMonitor.SetPlayerLastSeen(dataModel.WorldId, dataModel.CharacterId, dataModel.Timestamp, dataModel.ZoneId);
        }

        [CensusEventHandler("FacilityControl", typeof(FacilityControl))]
        private async Task Process(FacilityControl payload)
        {
            await _facilityControlSemaphore.WaitAsync();

            float scoreVs = 0f;
            float scoreNc = 0f;
            float scoreTr = 0f;
            int popVs = 0;
            int popNc = 0;
            int popTr = 0;

            try
            {
                var zonePopulation = _worldMonitor.GetZonePopulation(payload.WorldId, payload.ZoneId.Value);
                var mapUpdate = await _worldMonitor.UpdateFacilityControl(payload);

                var score = mapUpdate?.Score;

                if (score != null)
                {
                    scoreVs = score.ConnectedTerritories.Vs.Percent * 100;
                    scoreNc = score.ConnectedTerritories.Nc.Percent * 100;
                    scoreTr = score.ConnectedTerritories.Tr.Percent * 100;
                }

                if (zonePopulation != null)
                {
                    popVs = zonePopulation.VS;
                    popNc = zonePopulation.NC;
                    popTr = zonePopulation.TR;
                }

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
                    ZoneControlVs = scoreVs,
                    ZoneControlNc = scoreNc,
                    ZoneControlTr = scoreTr,
                    ZonePopulationVs = popVs,
                    ZonePopulationNc = popNc,
                    ZonePopulationTr = popTr
                };

                await _eventRepository.AddAsync(dataModel);

                if (dataModel.NewFactionId != dataModel.OldFactionId && score != null)
                {
                    var alert = await _alertRepository.GetActiveAlert(dataModel.WorldId, dataModel.ZoneId);
                    if (alert == null)
                    {
                        return;
                    }

                    if (alert.MetagameEvent?.Type == 1 || alert.MetagameEvent?.Type == 8 || alert.MetagameEvent?.Type == 9)
                    {
                        alert.LastFactionVs = score.ConnectedTerritories.Vs.Percent * 100;
                        alert.LastFactionNc = score.ConnectedTerritories.Nc.Percent * 100;
                        alert.LastFactionTr = score.ConnectedTerritories.Tr.Percent * 100;
                    }
                    else if (alert.MetagameEvent?.Type == 2 && alert.MetagameEventId == 9 && alert.MetagameEventId == 12 && alert.MetagameEventId == 14 && alert.MetagameEventId == 18)
                    {
                        alert.LastFactionVs = score.AmpStations.Vs.Value * 100;
                        alert.LastFactionNc = score.AmpStations.Nc.Value * 100;
                        alert.LastFactionTr = score.AmpStations.Tr.Value * 100;
                    }
                    else if (alert.MetagameEvent?.Type == 2 && alert.MetagameEventId == 8 && alert.MetagameEventId == 11 && alert.MetagameEventId == 17)
                    {
                        alert.LastFactionVs = score.TechPlants.Vs.Percent * 100;
                        alert.LastFactionNc = score.TechPlants.Nc.Percent * 100;
                        alert.LastFactionTr = score.TechPlants.Tr.Percent * 100;
                    }
                    else if (alert.MetagameEvent?.Type == 2 && alert.MetagameEventId == 7 && alert.MetagameEventId == 10 && alert.MetagameEventId == 13 && alert.MetagameEventId == 16)
                    {
                        alert.LastFactionVs = score.BioLabs.Vs.Percent * 100;
                        alert.LastFactionNc = score.BioLabs.Nc.Percent * 100;
                        alert.LastFactionTr = score.BioLabs.Tr.Percent * 100;
                    }
                    else
                    {
                        return;
                    }

                    await _alertRepository.UpdateAsync(alert);
                }
            }
            finally
            {
                _facilityControlSemaphore.Release();
            }
        }

        [CensusEventHandler("GainExperience", typeof(GainExperience))]
        private async Task Process(GainExperience payload)
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
            await _eventRepository.AddAsync(dataModel);
            _worldMonitor.SetPlayerLastSeen(dataModel.WorldId, dataModel.CharacterId, dataModel.Timestamp, dataModel.ZoneId);
        }

        [CensusEventHandler("MetagameEvent", typeof(MetagameEvent))]
        private async Task Process(MetagameEvent payload)
        {
            var dataModel = new Data.Models.Planetside.Events.MetagameEvent
            {
                InstanceId = payload.InstanceId,
                MetagameEventId = payload.MetagameEventId,
                MetagameEventState = payload.MetagameEventState,
                ZoneControlVs = payload.FactionVs,
                ZoneControlNc = payload.FactionNc,
                ZoneControlTr = payload.FactionTr,
                ExperienceBonus = (int)payload.ExperienceBonus,
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
        private async Task Proces(Models.PlayerFacilityCapture payload)
        {
            var dataModel = new Data.Models.Planetside.Events.PlayerFacilityCapture
            {
                FacilityId = payload.FacilityId,
                CharacterId = payload.CharacterId,
                OutfitId = payload.OutfitId == "0" ? null : payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };

            await _playerFacilityCaptureSemaphore.WaitAsync();

            try
            {
                await _eventRepository.AddAsync(dataModel);
                _worldMonitor.SetPlayerLastSeen(dataModel.WorldId, dataModel.CharacterId, dataModel.Timestamp, dataModel.ZoneId);
            }
            finally
            {
                _playerFacilityCaptureSemaphore.Release();
            }
        }

        [CensusEventHandler("PlayerFacilityDefend", typeof(PlayerFacilityDefend))]
        private async Task Process(PlayerFacilityDefend payload)
        {
            var dataModel = new Data.Models.Planetside.Events.PlayerFacilityDefend
            {
                FacilityId = payload.FacilityId,
                CharacterId = payload.CharacterId,
                OutfitId = payload.OutfitId == "0" ? null : payload.OutfitId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };

            await _playerFacilityDefendSemaphore.WaitAsync();

            try
            {
                await _eventRepository.AddAsync(dataModel);
                _worldMonitor.SetPlayerLastSeen(dataModel.WorldId, dataModel.CharacterId, dataModel.Timestamp, dataModel.ZoneId);
            }
            finally
            {
                _playerFacilityDefendSemaphore.Release();
            }
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
        private async Task Process(Models.VehicleDestroy payload)
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

            await _eventRepository.AddAsync(dataModel);
        }
    }
}
