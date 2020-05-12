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
using DaybreakGames.Census;
using DaybreakGames.Census.JsonConverters;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Core.Models;
using Voidwell.DaybreakGames.GameState.CensusStream.Models;
using Voidwell.DaybreakGames.Messaging;
using Voidwell.DaybreakGames.Messaging.Models;
using System.Collections.Concurrent;
using Voidwell.DaybreakGames.GameState.Services;
using Voidwell.DaybreakGames.Core.Services.Planetside;
using Voidwell.DaybreakGames.GameState.Models;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.GameState.CensusStream
{
    public class WebsocketEventHandler : IWebsocketEventHandler
    {
        private readonly IEventRepository _eventRepository;
        private readonly IAlertService _alertService;
        private readonly IWorldMonitor _worldMonitor;
        private readonly IPlayerMonitor _playerMonitor;
        private readonly ICharacterService _characterService;
        private readonly IMetagameEventMonitor _metagameEventMonitor;
        private readonly ICharacterRatingService _characterRatingService;
        private readonly IMetagameEventService _metagameEventService;
        private readonly IMessageService _messageService;
        private readonly ILogger<WebsocketEventHandler> _logger;
        private readonly Dictionary<string, MethodInfo> _processMethods;

        private readonly SemaphoreSlim _continentUnlockSemaphore;
        private readonly SemaphoreSlim _playerFacilityCaptureSemaphore;
        private readonly SemaphoreSlim _playerFacilityDefendSemaphore;
        private readonly SemaphoreSlim _facilityControlSemaphore;

        private const string RegServer = @"EventServerEndpoint_(.*)_(.*)";

        private readonly JsonSerializer _payloadDeserializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = new UnderscorePropertyNamesContractResolver(),
            Converters = new JsonConverter[]
                {
                    new BooleanJsonConverter(),
                    new DateTimeJsonConverter()
                }
        });

        public WebsocketEventHandler(IEventRepository eventRepository, IAlertService alertService, IWorldMonitor worldMonitor,
            IPlayerMonitor playerMonitor, ICharacterService characterService, IMetagameEventMonitor metagameEventMonitor,
            ICharacterRatingService characterRatingService, IMetagameEventService metagameEventService,
            IMessageService messageService, ILogger<WebsocketEventHandler> logger)
        {
            _eventRepository = eventRepository;
            _alertService = alertService;
            _worldMonitor = worldMonitor;
            _playerMonitor = playerMonitor;
            _characterService = characterService;
            _metagameEventMonitor = metagameEventMonitor;
            _characterRatingService = characterRatingService;
            _metagameEventService = metagameEventService;
            _messageService = messageService;
            _logger = logger;

            _processMethods = GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttribute<CensusEventHandlerAttribute>() != null)
                .ToDictionary(m => m.GetCustomAttribute<CensusEventHandlerAttribute>().EventName);

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

            var r = new Regex(RegServer);
            var m = r.Match(detail);

            var worldName = m.Groups[1].Value;

            if (int.TryParse(m.Groups[2].Value, out var worldId))
            {
                var isWorldOnline = message.Value<bool>("online");

                await _worldMonitor.SetWorldState(worldId, worldName, isWorldOnline);

                if (isWorldOnline)
                {
                    var activeAlerts = await _alertService.GetActiveAlertsByWorldId(worldId);
                    if (activeAlerts != null)
                    {
                        foreach(var alert in activeAlerts)
                        {
                            var metagameEvent = await _metagameEventService.GetMetagameEvent(alert.MetagameEventId.Value);
                            var alertState = new ZoneAlertState(alert.StartDate.Value, alert.MetagameInstanceId, metagameEvent);
                            _worldMonitor.UpdateZoneAlert(alert.WorldId, alert.ZoneId.Value, alertState);
                        }
                    }
                }
            }
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

        [CensusEventHandler("AchievementEarned", typeof(AchievementEarned))]
        private Task Process(AchievementEarned payload)
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
            await Task.WhenAll(_eventRepository.AddAsync(dataModel), _playerMonitor.SetLastSeenAsync(dataModel.CharacterId, dataModel.ZoneId, dataModel.Timestamp));
        }

        [CensusEventHandler("ContinentLock", typeof(ContinentLock))]
        private Task Process(ContinentLock payload)
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

            _worldMonitor.UpdateZoneLock(model.WorldId, model.ZoneId, new ZoneLockState(model.Timestamp));

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

        [CensusEventHandler("Death", typeof(Death))]
        private async Task Process(Death payload)
        {
            var TaskList = new List<Task>();
            Task<OutfitMember> AttackerOutfitTask = null;
            Task<OutfitMember> VictimOutfitTask = null;

            if (payload.AttackerCharacterId != null && payload.AttackerCharacterId.Length > 18)
            {
                AttackerOutfitTask = _characterService.GetCharactersOutfit(payload.AttackerCharacterId);
                TaskList.Add(AttackerOutfitTask);
            }

            if (payload.CharacterId != null && payload.CharacterId.Length > 18)
            {
                VictimOutfitTask = _characterService.GetCharactersOutfit(payload.CharacterId);
                TaskList.Add(VictimOutfitTask);
            }

            if (payload.AttackerCharacterId != null && payload.CharacterId != null &&
                payload.AttackerCharacterId != payload.CharacterId &&
                payload.AttackerCharacterId.Length > 18 && payload.CharacterId.Length > 18)
            {
                TaskList.Add(_characterRatingService.CalculateRatingAsync(payload.AttackerCharacterId, payload.CharacterId));
            }

            await Task.WhenAll(TaskList);

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

            var attackerTask = _playerMonitor.SetLastSeenAsync(payload.AttackerCharacterId, payload.ZoneId.Value, payload.Timestamp);
            var victimTask = _playerMonitor.SetLastSeenAsync(payload.CharacterId, payload.ZoneId.Value, payload.Timestamp);

            await Task.WhenAll(_eventRepository.AddAsync(dataModel), attackerTask, victimTask);

            var messageTask = SendPlayerDeathMessage(attackerTask.Result, victimTask.Result, dataModel);
        }

        [CensusEventHandler("FacilityControl", typeof(FacilityControl))]
        private async Task Process(FacilityControl payload)
        {
            await _facilityControlSemaphore.WaitAsync();

            var scoreVs = 0f;
            var scoreNc = 0f;
            var scoreTr = 0f;
            var scoreNs = 0f;
            var popVs = 0;
            var popNc = 0;
            var popTr = 0;
            var popNs = 0;

            try
            {
                var zonePopulationTask = _worldMonitor.GetZonePopulation(payload.WorldId, payload.ZoneId.Value);
                var mapUpdateTask = _worldMonitor.UpdateFacilityControl(payload);

                await Task.WhenAll(zonePopulationTask, mapUpdateTask);

                var zonePopulation = zonePopulationTask.Result;
                var mapUpdate = mapUpdateTask.Result;

                var score = mapUpdate?.Score;

                if (score != null)
                {
                    scoreVs = score.ConnectedTerritories.Vs.Percent * 100;
                    scoreNc = score.ConnectedTerritories.Nc.Percent * 100;
                    scoreTr = score.ConnectedTerritories.Tr.Percent * 100;
                    scoreNs = score.ConnectedTerritories.Ns.Percent * 100;
                }

                if (zonePopulation != null)
                {
                    popVs = zonePopulation.VS;
                    popNc = zonePopulation.NC;
                    popTr = zonePopulation.TR;
                    popNs = zonePopulation.NS;
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
                    ZoneControlNs = scoreNs,
                    ZonePopulationVs = popVs,
                    ZonePopulationNc = popNc,
                    ZonePopulationTr = popTr,
                    ZonePopulationNs = popNs
                };

                await _eventRepository.AddAsync(dataModel);

                if (dataModel.NewFactionId != dataModel.OldFactionId && score != null)
                {
                    var alert = await _alertService.GetActiveAlert(dataModel.WorldId, dataModel.ZoneId);
                    if (alert == null)
                    {
                        return;
                    }

                    if (alert.MetagameEvent?.Type == 1 || alert.MetagameEvent?.Type == 8 || alert.MetagameEvent?.Type == 9)
                    {
                        alert.LastFactionVs = score.ConnectedTerritories.Vs.Percent * 100;
                        alert.LastFactionNc = score.ConnectedTerritories.Nc.Percent * 100;
                        alert.LastFactionTr = score.ConnectedTerritories.Tr.Percent * 100;
                        alert.LastFactionNs = score.ConnectedTerritories.Ns.Percent * 100;
                    }
                    else if (alert.MetagameEventId == 9 || alert.MetagameEventId == 12 || alert.MetagameEventId == 14 || alert.MetagameEventId == 18)
                    {
                        alert.LastFactionVs = score.AmpStations.Vs.Value;
                        alert.LastFactionNc = score.AmpStations.Nc.Value;
                        alert.LastFactionTr = score.AmpStations.Tr.Value;
                        alert.LastFactionNs = score.AmpStations.Ns.Value;
                    }
                    else if (alert.MetagameEventId == 8 || alert.MetagameEventId == 11 || alert.MetagameEventId == 17)
                    {
                        alert.LastFactionVs = score.TechPlants.Vs.Value;
                        alert.LastFactionNc = score.TechPlants.Nc.Value;
                        alert.LastFactionTr = score.TechPlants.Tr.Value;
                        alert.LastFactionNs = score.TechPlants.Ns.Value;
                    }
                    else if (alert.MetagameEventId == 7 || alert.MetagameEventId == 10 || alert.MetagameEventId == 13 || alert.MetagameEventId == 16)
                    {
                        alert.LastFactionVs = score.BioLabs.Vs.Value;
                        alert.LastFactionNc = score.BioLabs.Nc.Value;
                        alert.LastFactionTr = score.BioLabs.Tr.Value;
                        alert.LastFactionNs = score.BioLabs.Ns.Value;
                    }
                    else if (alert.MetagameEventId == 180 || alert.MetagameEventId == 181 || alert.MetagameEventId == 182 || alert.MetagameEventId == 183)
                    {
                        alert.LastFactionVs = score.LargeOutposts.Vs.Value;
                        alert.LastFactionNc = score.LargeOutposts.Nc.Value;
                        alert.LastFactionTr = score.LargeOutposts.Tr.Value;
                        alert.LastFactionNs = score.LargeOutposts.Ns.Value;
                    }
                    else
                    {
                        return;
                    }

                    await _alertService.UpdateAlert(alert);
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
                Id = Guid.NewGuid(),
                ExperienceId = payload.ExperienceId,
                CharacterId = payload.CharacterId,
                Amount = payload.Amount,
                LoadoutId = payload.LoadoutId,
                OtherId = payload.OtherId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };

            await Task.WhenAll(_eventRepository.AddAsync(dataModel), _playerMonitor.SetLastSeenAsync(dataModel.CharacterId, dataModel.ZoneId, dataModel.Timestamp));
        }

        private static int[] EsamirLockingMetagameEventIds = { 2, 126, 127, 128, 150, 151, 152, 176, 186, 190 };

        [CensusEventHandler("MetagameEvent", typeof(MetagameEvent))]
        private async Task Process(MetagameEvent payload)
        {
            // Daybreak reset their instance_id counter
            payload.InstanceId = int.Parse($"{payload.InstanceId}18");

            var metagameCategory = await _metagameEventService.GetMetagameEvent(payload.MetagameEventId);

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
                ZoneId = payload.ZoneId ?? metagameCategory.ZoneId
            };

            //Bypass Esamir not returning continent lock/unlock events.
            /*
            if (dataModel.MetagameEventState == "135" && dataModel.MetagameEventId == 160)
            {
                var unlockPayload = new ContinentUnlock
                {
                    Timestamp = dataModel.Timestamp,
                    EventName = "ContinentUnlock",
                    MetagameEventId = dataModel.MetagameEventId.GetValueOrDefault(),
                    TriggeringFaction = 0,
                    WorldId = dataModel.WorldId,
                    ZoneId = dataModel.ZoneId
                };
                await Process(unlockPayload);
            }
            else if (dataModel.MetagameEventState == "138" && EsamirLockingMetagameEventIds.Contains(dataModel.MetagameEventId.GetValueOrDefault()))
            {
                var maxZoneControl = Math.Max(Math.Max(dataModel.ZoneControlVs.GetValueOrDefault(), dataModel.ZoneControlNc.GetValueOrDefault()), dataModel.ZoneControlTr.GetValueOrDefault());
                var winner = maxZoneControl == dataModel.ZoneControlVs ? 1 : (maxZoneControl == dataModel.ZoneControlNc ? 2 : 3);

                var lockPayload = new ContinentLock
                {
                    Timestamp = dataModel.Timestamp,
                    EventName = "ContinentLock",
                    MetagameEventId = dataModel.MetagameEventId.GetValueOrDefault(),
                    TriggeringFaction = winner,
                    WorldId = dataModel.WorldId,
                    ZoneId = dataModel.ZoneId,
                    VsPopulation = dataModel.ZoneControlVs.GetValueOrDefault(),
                    NcPopulation = dataModel.ZoneControlNc.GetValueOrDefault(),
                    TrPopulation = dataModel.ZoneControlTr.GetValueOrDefault()
                    
                };
                await Process(lockPayload);
            }
            */

            await Task.WhenAll(_eventRepository.AddAsync(dataModel), _metagameEventMonitor.ProcessMetagameEvent(payload));
        }

        [CensusEventHandler("PlayerFacilityCapture", typeof(PlayerFacilityCapture))]
        private async Task Proces(PlayerFacilityCapture payload)
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
                await Task.WhenAll(_eventRepository.AddAsync(dataModel), _playerMonitor.SetLastSeenAsync(dataModel.CharacterId, dataModel.ZoneId, dataModel.Timestamp));
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
                await Task.WhenAll(_eventRepository.AddAsync(dataModel), _playerMonitor.SetLastSeenAsync(dataModel.CharacterId, dataModel.ZoneId, dataModel.Timestamp));
            }
            finally
            {
                _playerFacilityDefendSemaphore.Release();
            }
        }

        [CensusEventHandler("PlayerLogin", typeof(PlayerLogin))]
        private async Task Process(PlayerLogin payload)
        {
            if (!await ValidateEvent(payload, a => a.CharacterId, a => DateTime.UtcNow - a.Timestamp > TimeSpan.FromSeconds(1)))
            {
                return;
            }

            var dataModel = new Data.Models.Planetside.Events.PlayerLogin
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };

            await Task.WhenAll(_eventRepository.AddAsync(dataModel), _playerMonitor.SetOnlineAsync(payload.CharacterId, payload.Timestamp));
        }

        [CensusEventHandler("PlayerLogout", typeof(PlayerLogout))]
        private async Task Process(PlayerLogout payload)
        {
            if (!await ValidateEvent(payload, a => a.CharacterId, a => DateTime.UtcNow - a.Timestamp > TimeSpan.FromSeconds(1)))
            {
                return;
            }

            var dataModel = new Data.Models.Planetside.Events.PlayerLogout
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };

            await Task.WhenAll(_eventRepository.AddAsync(dataModel), _playerMonitor.SetOfflineAsync(payload.CharacterId, payload.Timestamp));
        }

        [CensusEventHandler("VehicleDestroy", typeof(VehicleDestroy))]
        private async Task Process(VehicleDestroy payload)
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

        private readonly ConcurrentDictionary<string, object> _eventBuffer = new ConcurrentDictionary<string, object>();
        private readonly KeyedSemaphoreSlim _eventSemaphore = new KeyedSemaphoreSlim();

        private async Task<bool> ValidateEvent<T>(T ev, Func<T, string> keyExpr, Func<T, bool> cleanupExpr) where T: class
        {
            var eventKey = $"{typeof(T).Name}:{keyExpr(ev)}";
            using (await _eventSemaphore.WaitAsync(eventKey))
            {
                var isValid = !_eventBuffer.ContainsKey(eventKey);

                var expiredKeys = _eventBuffer.Keys.ToList()
                    .Where(k => _eventBuffer.TryGetValue(k, out var value) && value is T  && cleanupExpr(value as T)).ToList();
                expiredKeys.ForEach(k => _eventBuffer.TryRemove(k, out var value));

                _eventBuffer.TryAdd(eventKey, ev);

                return isValid;
            }
        }

        private async Task SendPlayerDeathMessage(OnlineCharacter attackerCharacter, OnlineCharacter victimCharacter, Data.Models.Planetside.Events.Death model)
        {
            var message = new PlayerDeathMessage
            {
                AttackerCharacterId = attackerCharacter?.Character?.CharacterId,
                AttackerFactionId = attackerCharacter?.Character?.FactionId,
                AttackerCharacterName = attackerCharacter?.Character?.Name,
                AttackerOutfitId = model.AttackerOutfitId,
                VictimCharacterId = victimCharacter?.Character?.CharacterId,
                VictimFactionId = victimCharacter?.Character?.FactionId,
                VictimCharacterName = victimCharacter?.Character?.Name,
                VictimOutfitId = model.CharacterOutfitId,
                AttackerWeaponId = model.AttackerWeaponId,
                IsHeadshot = model.IsHeadshot,
                AttackerVehicleId = model.AttackerVehicleId,
                ZoneId = model.ZoneId,
                Timestamp = model.Timestamp,
                WorldId = model.WorldId
            };

            var messageTasks = new List<Task>();

            if (attackerCharacter != null)
            {
                messageTasks.Add(_messageService.PublishCharacterEvent(message.AttackerCharacterId, message));
            }

            if (victimCharacter != null)
            {
                messageTasks.Add(_messageService.PublishCharacterEvent(message.VictimCharacterId, message));
            }

            if (messageTasks.Any())
            {
                await Task.WhenAll(messageTasks);
            }
        }

        public void Dispose()
        {
            _continentUnlockSemaphore.Dispose();
            _playerFacilityCaptureSemaphore.Dispose();
            _playerFacilityDefendSemaphore.Dispose();
            _facilityControlSemaphore.Dispose();
        }
    }
}
