using Voidwell.DaybreakGames.Live.Mappers;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;

[CensusEventProcessor("BattleRankUp")]
public class BattlerankUpProcessor : IEventProcessor<BattlerankUp>
{
    private readonly IEventRepository _eventRepository;
    private readonly IPlayerMonitor _playerMonitor;

    public BattlerankUpProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor)
    {
        _eventRepository = eventRepository;
        _playerMonitor = playerMonitor;
    }

    public async Task Process(BattlerankUp payload)
    {
        var model = CensusToDataMapper.Map(payload);
        await Task.WhenAll(_eventRepository.AddAsync(model), _playerMonitor.SetLastSeenAsync(model.CharacterId!, model.ZoneId, model.Timestamp));
    }
}
