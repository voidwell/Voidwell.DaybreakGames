using Voidwell.DaybreakGames.Live.Mappers;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Live.CensusStream.Models;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;

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
        var model = CensusToDataMapper.Map(payload);
        return _eventRepository.AddAsync(model);
    }
}
