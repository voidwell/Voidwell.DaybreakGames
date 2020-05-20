using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;
using Voidwell.DaybreakGames.Services.Models;

namespace Voidwell.DaybreakGames.CensusStream.EventProcessors
{
    [CensusEventProcessor("ContinentUnlock")]
    public class ContinentUnlockProcessor : IEventProcessor<ContinentUnlock>, IDisposable
    {
        private readonly IEventRepository _eventRepository;
        private readonly IWorldMonitor _worldMonitor;

        private readonly SemaphoreSlim _continentUnlockSemaphore = new SemaphoreSlim(1);

        public ContinentUnlockProcessor(IEventRepository eventRepository, IWorldMonitor worldMonitor)
        {
            _eventRepository = eventRepository;
            _worldMonitor = worldMonitor;
        }

        public async Task Process(ContinentUnlock payload)
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

        public void Dispose()
        {
            _continentUnlockSemaphore.Dispose();
        }
    }
}
