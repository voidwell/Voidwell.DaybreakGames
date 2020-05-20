using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStream;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.CensusStream.EventProcessors
{
    [CensusEventProcessor("PlayerLogin")]
    public class PlayerLoginProcessor : IEventProcessor<PlayerLogin>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPlayerMonitor _playerMonitor;
        private readonly IEventValidator _eventValidator;

        public PlayerLoginProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor, IEventValidator eventValidator)
        {
            _eventRepository = eventRepository;
            _playerMonitor = playerMonitor;
            _eventValidator = eventValidator;
        }

        public async Task Process(PlayerLogin payload)
        {
            if (!await ValidateEvent(payload))
            {
                return;
            }

            var dataModel = new Data.Models.Planetside.Events.PlayerLogin
            {
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId
            };

            await Task.WhenAll(_eventRepository.AddAsync(dataModel), _playerMonitor.SetOnlineAsync(payload.CharacterId, payload.Timestamp));
        }

        private Task<bool> ValidateEvent(PlayerLogin payload)
        {
            return _eventValidator.Validiate(payload, a => a.CharacterId, a => DateTime.UtcNow - a.Timestamp > TimeSpan.FromSeconds(1));
        }
    }
}
