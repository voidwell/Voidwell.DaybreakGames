using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Services.Models;

namespace Voidwell.DaybreakGames.CensusStream.EventProcessors
{
    [CensusEventProcessor("ContinentLock")]
    public class ContinentLockProcessor : IEventProcessor<ContinentLock>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IWorldMonitor _worldMonitor;

        public ContinentLockProcessor(IEventRepository eventRepository, IWorldMonitor worldMonitor)
        {
            _eventRepository = eventRepository;
            _worldMonitor = worldMonitor;
        }

        public async Task Process(ContinentLock payload)
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

            await _eventRepository.AddAsync(model);
        }
    }
}
