using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Live.Mappers;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;

[CensusEventProcessor("PlayerFacilityDefend")]
public class PlayerFacilityDefendProcessor : IEventProcessor<PlayerFacilityDefend>, IDisposable
{
    private readonly IEventRepository _eventRepository;
    private readonly IPlayerMonitor _playerMonitor;

    private readonly SemaphoreSlim _playerFacilityDefendSemaphore = new SemaphoreSlim(5);

    public PlayerFacilityDefendProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor)
    {
        _eventRepository = eventRepository;
        _playerMonitor = playerMonitor;
    }

    public async Task Process(PlayerFacilityDefend payload)
    {
        var model = CensusToDataMapper.Map(payload);

        await _playerFacilityDefendSemaphore.WaitAsync();

        try
        {
            await Task.WhenAll(_eventRepository.AddAsync(model), _playerMonitor.SetLastSeenAsync(model.CharacterId!, model.ZoneId, model.Timestamp));
        }
        finally
        {
            _playerFacilityDefendSemaphore.Release();
        }
    }

    public void Dispose()
    {
        _playerFacilityDefendSemaphore.Dispose();
    }
}
