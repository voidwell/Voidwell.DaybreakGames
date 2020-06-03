using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.CensusStream.EventProcessors
{
    [CensusEventProcessor("FacilityControl")]
    public class FacilityControlProcessor : IEventProcessor<FacilityControl>, IDisposable
    {
        private readonly IEventRepository _eventRepository;
        private readonly IAlertRepository _alertRepository;
        private readonly IWorldMonitor _worldMonitor;

        private readonly SemaphoreSlim _facilityControlSemaphore = new SemaphoreSlim(3);

        public FacilityControlProcessor(IEventRepository eventRepository, IAlertRepository alertRepository, IWorldMonitor worldMonitor)
        {
            _eventRepository = eventRepository;
            _alertRepository = alertRepository;
            _worldMonitor = worldMonitor;
        }

        public async Task Process(FacilityControl payload)
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

                    await _alertRepository.UpdateAsync(alert);
                }
            }
            finally
            {
                _facilityControlSemaphore.Release();
            }
        }

        public void Dispose()
        {
            _facilityControlSemaphore.Dispose();
        }
    }
}
