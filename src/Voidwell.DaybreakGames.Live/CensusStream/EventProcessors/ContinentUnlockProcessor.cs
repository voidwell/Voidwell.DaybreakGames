using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Live.Mappers;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;

[CensusEventProcessor("ContinentUnlock")]
public class ContinentUnlockProcessor : IEventProcessor<ContinentUnlock>, IDisposable
{
    private readonly IEventRepository _eventRepository;
    private readonly IWorldMonitor _worldMonitor;

    private readonly SemaphoreSlim _continentUnlockSemaphore = new SemaphoreSlim(1);

    public ContinentUnlockProcessor(IEventRepository eventRepository, IWorldMonitor worldMonitor)
    {
        _eventRepository = eventRepository;
        _worldMonitor = worldMonitor;
    }

    public async Task Process(ContinentUnlock payload)
    {
        var model = CensusToDataMapper.Map(payload);

        _worldMonitor.UpdateZoneLock(model.WorldId, model.ZoneId, new ZoneLockState(model.Timestamp));

        await _continentUnlockSemaphore.WaitAsync();

        try
        {
            await _eventRepository.AddAsync(model);
        }
        finally
        {
            _continentUnlockSemaphore.Release();
        }
    }

    public void Dispose()
    {
        _continentUnlockSemaphore.Dispose();
    }
}
