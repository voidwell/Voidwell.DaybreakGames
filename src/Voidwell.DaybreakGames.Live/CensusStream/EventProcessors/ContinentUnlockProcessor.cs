using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Live.GameState;
using AutoMapper;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    [CensusEventProcessor("ContinentUnlock")]
    public class ContinentUnlockProcessor : IEventProcessor<ContinentUnlock>, IDisposable
    {
        private readonly IEventRepository _eventRepository;
        private readonly IWorldMonitor _worldMonitor;
        private readonly IMapper _mapper;

        private readonly SemaphoreSlim _continentUnlockSemaphore = new SemaphoreSlim(1);

        public ContinentUnlockProcessor(IEventRepository eventRepository, IWorldMonitor worldMonitor, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _worldMonitor = worldMonitor;
            _mapper = mapper;
        }

        public async Task Process(ContinentUnlock payload)
        {
            var model = _mapper.Map<Data.Models.Planetside.Events.ContinentUnlock>(payload);

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

        public void Dispose()
        {
            _continentUnlockSemaphore.Dispose();
        }
    }
}
