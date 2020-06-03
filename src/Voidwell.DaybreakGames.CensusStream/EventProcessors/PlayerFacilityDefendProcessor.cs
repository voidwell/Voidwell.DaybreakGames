using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.CensusStream.EventProcessors
{
    [CensusEventProcessor("PlayerFacilityDefend")]
    public class PlayerFacilityDefendProcessor : IEventProcessor<PlayerFacilityDefend>, IDisposable
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPlayerMonitor _playerMonitor;

        private readonly SemaphoreSlim _playerFacilityDefendSemaphore = new SemaphoreSlim(5);

        public PlayerFacilityDefendProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor)
        {
            _eventRepository = eventRepository;
            _playerMonitor = playerMonitor;
        }

        public async Task Process(PlayerFacilityDefend payload)
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

        public void Dispose()
        {
            _playerFacilityDefendSemaphore.Dispose();
        }
    }
}
