using Voidwell.DaybreakGames.Live.Mappers;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Live.CensusStream.Models;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;

[CensusEventProcessor("VehicleDestroy")]
public class VehicleDestroyProcessor : IEventProcessor<VehicleDestroy>
{
    private readonly IEventRepository _eventRepository;

    public VehicleDestroyProcessor(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task Process(VehicleDestroy payload)
    {
        var dataModel = CensusToDataMapper.Map(payload);
        await _eventRepository.AddAsync(dataModel);
    }
}
