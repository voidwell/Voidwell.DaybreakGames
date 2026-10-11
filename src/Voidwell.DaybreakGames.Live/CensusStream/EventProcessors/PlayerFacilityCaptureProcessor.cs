using Voidwell.DaybreakGames.Live.Mappers;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;

[CensusEventProcessor("PlayerFacilityCapture")]
public class PlayerFacilityCaptureProcessor : IEventProcessor<PlayerFacilityCapture>, IDisposable
{
    private readonly IEventRepository _eventRepository;
    private readonly IPlayerMonitor _playerMonitor;

    private readonly SemaphoreSlim _playerFacilityCaptureSemaphore = new SemaphoreSlim(5);

    public PlayerFacilityCaptureProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor)
    {
        _eventRepository = eventRepository;
        _playerMonitor = playerMonitor;
    }

    public async Task Process(PlayerFacilityCapture payload)
    {
        var model = CensusToDataMapper.Map(payload);

        await _playerFacilityCaptureSemaphore.WaitAsync();

        try
        {
            await Task.WhenAll(_eventRepository.AddAsync(model), _playerMonitor.SetLastSeenAsync(model.CharacterId!, model.ZoneId, model.Timestamp));
        }
        finally
        {
            _playerFacilityCaptureSemaphore.Release();
        }
    }

    public void Dispose()
    {
        _playerFacilityCaptureSemaphore.Dispose();
    }
}
