using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Live.Mappers;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;

[CensusEventProcessor("GainExperience")]
public class GainExperienceProcessor : IEventProcessor<GainExperience>
{
    private readonly IEventRepository _eventRepository;
    private readonly IPlayerMonitor _playerMonitor;

    public GainExperienceProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor)
    {
        _eventRepository = eventRepository;
        _playerMonitor = playerMonitor;
    }

    public async Task Process(GainExperience payload)
    {
        var model = CensusToDataMapper.Map(payload);

        await Task.WhenAll(_eventRepository.AddAsync(model), _playerMonitor.SetLastSeenAsync(model.CharacterId!, model.ZoneId, model.Timestamp));
    }
}
