using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using AutoMapper;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    [CensusEventProcessor("AchievementEarned")]
    public class AchievementEarnedProcessor : IEventProcessor<AchievementEarned>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public AchievementEarnedProcessor(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public Task Process(AchievementEarned payload)
        {
            var model = _mapper.Map<Data.Models.Planetside.Events.AchievementEarned>(payload);
            return _eventRepository.AddAsync(model);
        }
    }
}
