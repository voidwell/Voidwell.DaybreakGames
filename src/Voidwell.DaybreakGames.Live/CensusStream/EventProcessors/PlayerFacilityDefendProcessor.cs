using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using AutoMapper;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    [CensusEventProcessor("PlayerFacilityDefend")]
    public class PlayerFacilityDefendProcessor : IEventProcessor<PlayerFacilityDefend>, IDisposable
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPlayerMonitor _playerMonitor;
        private readonly IMapper _mapper;

        private readonly SemaphoreSlim _playerFacilityDefendSemaphore = new SemaphoreSlim(5);

        public PlayerFacilityDefendProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _playerMonitor = playerMonitor;
            _mapper = mapper;
        }

        public async Task Process(PlayerFacilityDefend payload)
        {
            var model = _mapper.Map<Data.Models.Planetside.Events.PlayerFacilityDefend>(payload);

            await _playerFacilityDefendSemaphore.WaitAsync();

            try
            {
                await Task.WhenAll(_eventRepository.AddAsync(model), _playerMonitor.SetLastSeenAsync(model.CharacterId, model.ZoneId, model.Timestamp));
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
