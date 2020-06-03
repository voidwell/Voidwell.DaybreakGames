using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.CensusStream.EventProcessors
{
    [CensusEventProcessor("AchievementEarned")]
    public class AchievementEarnedProcessor : IEventProcessor<AchievementEarned>
    {
        private readonly IEventRepository _eventRepository;

        public AchievementEarnedProcessor(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public Task Process(AchievementEarned payload)
        {
            var dataModel = new Data.Models.Planetside.Events.AchievementEarned
            {
                AchievementId = payload.AchievementId,
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };
            return _eventRepository.AddAsync(dataModel);
        }
    }
}
