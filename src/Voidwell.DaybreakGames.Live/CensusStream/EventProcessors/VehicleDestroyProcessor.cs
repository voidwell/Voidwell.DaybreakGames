using System.Threading.Tasks;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using AutoMapper;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors
{
    [CensusEventProcessor("VehicleDestroy")]
    public class VehicleDestroyProcessor : IEventProcessor<VehicleDestroy>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public VehicleDestroyProcessor(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task Process(VehicleDestroy payload)
        {
            var dataModel = _mapper.Map<Data.Models.Planetside.Events.VehicleDestroy>(payload);
            await _eventRepository.AddAsync(dataModel);
        }
    }
}
