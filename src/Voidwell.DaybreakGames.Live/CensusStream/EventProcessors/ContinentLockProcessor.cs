using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Live.GameState;
using AutoMapper;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    [CensusEventProcessor("ContinentLock")]
    public class ContinentLockProcessor : IEventProcessor<ContinentLock>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IWorldMonitor _worldMonitor;
        private readonly IMapper _mapper;

        public ContinentLockProcessor(IEventRepository eventRepository, IWorldMonitor worldMonitor, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _worldMonitor = worldMonitor;
            _mapper = mapper;
        }

        public async Task Process(ContinentLock payload)
        {
            var model = _mapper.Map<Data.Models.Planetside.Events.ContinentLock>(payload);

            _worldMonitor.UpdateZoneLock(model.WorldId, model.ZoneId, new ZoneLockState(model.Timestamp, model.MetagameEventId, model.TriggeringFaction));

            await _eventRepository.AddAsync(model);
        }
    }
}
