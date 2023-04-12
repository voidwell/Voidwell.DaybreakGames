using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using AutoMapper;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    [CensusEventProcessor("BattleRankUp")]
    public class BattlerankUpProcessor : IEventProcessor<BattlerankUp>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPlayerMonitor _playerMonitor;
        private readonly IMapper _mapper;

        public BattlerankUpProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _playerMonitor = playerMonitor;
            _mapper = mapper;
        }

        public async Task Process(BattlerankUp payload)
        {
            var model = _mapper.Map<Data.Models.Planetside.Events.BattlerankUp>(payload);
            await Task.WhenAll(_eventRepository.AddAsync(model), _playerMonitor.SetLastSeenAsync(model.CharacterId, model.ZoneId, model.Timestamp));
        }
    }
}
