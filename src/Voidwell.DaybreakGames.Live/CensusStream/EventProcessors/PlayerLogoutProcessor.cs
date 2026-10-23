using System.Globalization;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Live.Mappers;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;

[CensusEventProcessor("PlayerLogout")]
public class PlayerLogoutProcessor : IEventProcessor<PlayerLogout>
{
    private readonly IEventRepository _eventRepository;
    private readonly IPlayerMonitor _playerMonitor;
    private readonly IEventValidator _eventValidator;

    public PlayerLogoutProcessor(IEventRepository eventRepository, IPlayerMonitor playerMonitor,
        IEventValidator eventValidator)
    {
        _eventRepository = eventRepository;
        _playerMonitor = playerMonitor;
        _eventValidator = eventValidator;
    }

    public async Task Process(PlayerLogout payload)
    {
        if (!await ValidateEvent(payload))
        {
            return;
        }

        var model = CensusToDataMapper.Map(payload);

        await Task.WhenAll(_eventRepository.AddAsync(model), _playerMonitor.SetOfflineAsync(payload.CharacterId!, payload.Timestamp));
    }

    private Task<bool> ValidateEvent(PlayerLogout payload)
    {
        return _eventValidator.Validiate(payload, a => a.CharacterId.ToString(CultureInfo.InvariantCulture), a => DateTime.UtcNow - a.Timestamp > TimeSpan.FromSeconds(1));
    }
}
