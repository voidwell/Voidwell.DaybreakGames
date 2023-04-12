using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using AutoMapper;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    [CensusEventProcessor("FacilityControl")]
    public class FacilityControlProcessor : IEventProcessor<FacilityControl>, IDisposable
    {
        private readonly IEventRepository _eventRepository;
        private readonly IAlertRepository _alertRepository;
        private readonly IWorldMonitor _worldMonitor;
        private readonly IMapper _mapper;

        private readonly SemaphoreSlim _facilityControlSemaphore = new SemaphoreSlim(3);

        public FacilityControlProcessor(IEventRepository eventRepository, IAlertRepository alertRepository,
            IWorldMonitor worldMonitor, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _alertRepository = alertRepository;
            _worldMonitor = worldMonitor;
            _mapper = mapper;
        }

        public async Task Process(FacilityControl payload)
        {
            await _facilityControlSemaphore.WaitAsync();

            try
            {
                var zonePopulationTask = _worldMonitor.GetZonePopulation(payload.WorldId, payload.ZoneId.Value);
                var mapUpdateTask = _worldMonitor.UpdateFacilityControl(payload);

                await Task.WhenAll(zonePopulationTask, mapUpdateTask);

                var zonePopulation = zonePopulationTask.Result;
                var mapUpdate = mapUpdateTask.Result;

                var score = mapUpdate?.Score;

                var model = _mapper.Map<Data.Models.Planetside.Events.FacilityControl>(payload);

                model.ZoneControlVs = score?.ConnectedTerritories.Vs.Percent * 100 ?? 0f;
                model.ZoneControlNc = score?.ConnectedTerritories.Nc.Percent * 100 ?? 0f;
                model.ZoneControlTr = score?.ConnectedTerritories.Tr.Percent * 100 ?? 0f;
                model.ZoneControlNs = score?.ConnectedTerritories.Ns.Percent * 100 ?? 0f;

                model.ZonePopulationVs = zonePopulation?.VS ?? 0;
                model.ZonePopulationNc = zonePopulation?.NC ?? 0;
                model.ZonePopulationTr = zonePopulation?.TR ?? 0;
                model.ZonePopulationNs = zonePopulation?.NS ?? 0;

                await Task.WhenAll(_eventRepository.AddAsync(model), UpdateAlertAsync(model, score));
            }
            finally
            {
                _facilityControlSemaphore.Release();
            }
        }

        private async Task UpdateAlertAsync(Data.Models.Planetside.Events.FacilityControl model, MapScore score)
        {
            if (model.NewFactionId == model.OldFactionId || score == null)
            {
                return;
            }

            var alert = await _alertRepository.GetActiveAlert(model.WorldId, model.ZoneId);
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

        public void Dispose()
        {
            _facilityControlSemaphore.Dispose();
        }
    }
}
