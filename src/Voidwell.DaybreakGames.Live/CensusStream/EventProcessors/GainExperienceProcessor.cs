using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using AutoMapper;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    [CensusEventProcessor("GainExperience")]
    public class GainExperienceProcessor : IEventProcessor<GainExperience>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IPlayerMonitor _playerMonitor;
        private readonly IMapper _mapper;

        public GainExperienceProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _playerMonitor = playerMonitor;
            _mapper = mapper;
        }

        public async Task Process(GainExperience payload)
        {
            var model = _mapper.Map<Data.Models.Planetside.Events.GainExperience>(payload);

            await Task.WhenAll(_eventRepository.AddAsync(model), _playerMonitor.SetLastSeenAsync(model.CharacterId, model.ZoneId, model.Timestamp));
        }
    }
}
