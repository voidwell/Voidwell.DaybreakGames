using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.CensusStream.EventProcessors
{
    [CensusEventProcessor("PlayerFacilityCapture")]
    public class PlayerFacilityCaptureProcessor : IEventProcessor<PlayerFacilityCapture>, IDisposable
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPlayerMonitor _playerMonitor;

        private readonly SemaphoreSlim _playerFacilityCaptureSemaphore = new SemaphoreSlim(5);

        public PlayerFacilityCaptureProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor)
        {
            _eventRepository = eventRepository;
            _playerMonitor = playerMonitor;
        }

        public async Task Process(PlayerFacilityCapture payload)
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

        public void Dispose()
        {
            _playerFacilityCaptureSemaphore.Dispose();
        }
    }
}
