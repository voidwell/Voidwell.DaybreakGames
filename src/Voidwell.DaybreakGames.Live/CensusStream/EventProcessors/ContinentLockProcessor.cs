using Voidwell.DaybreakGames.Live.Mappers;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;

[CensusEventProcessor("ContinentLock")]
public class ContinentLockProcessor : IEventProcessor<ContinentLock>
{
    private readonly IEventRepository _eventRepository;
    private readonly IWorldMonitor _worldMonitor;

    public ContinentLockProcessor(IEventRepository eventRepository, IWorldMonitor worldMonitor)
    {
        _eventRepository = eventRepository;
        _worldMonitor = worldMonitor;
    }

    public async Task Process(ContinentLock payload)
    {
        var model = CensusToDataMapper.Map(payload);

        _worldMonitor.UpdateZoneLock(model.WorldId, model.ZoneId, new ZoneLockState(model.Timestamp, model.MetagameEventId, model.TriggeringFaction));

        await _eventRepository.AddAsync(model);
    }
}
