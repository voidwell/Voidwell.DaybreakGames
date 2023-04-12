using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using AutoMapper;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    [CensusEventProcessor("PlayerLogin")]
    public class PlayerLoginProcessor : IEventProcessor<PlayerLogin>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPlayerMonitor _playerMonitor;
        private readonly IEventValidator _eventValidator;
        private readonly IMapper _mapper;

        public PlayerLoginProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor,
            IEventValidator eventValidator, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _playerMonitor = playerMonitor;
            _eventValidator = eventValidator;
            _mapper = mapper;
        }

        public async Task Process(PlayerLogin payload)
        {
            if (!await ValidateEvent(payload))
            {
                return;
            }

            var model = _mapper.Map<Data.Models.Planetside.Events.PlayerLogin>(payload);

            await Task.WhenAll(_eventRepository.AddAsync(model), _playerMonitor.SetOnlineAsync(payload.CharacterId, payload.Timestamp));
        }

        private Task<bool> ValidateEvent(PlayerLogin payload)
        {
            return _eventValidator.Validiate(payload, a => a.CharacterId, a => DateTime.UtcNow - a.Timestamp > TimeSpan.FromSeconds(1));
        }
    }
}
