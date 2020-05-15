using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStream;
using Voidwell.DaybreakGames.CensusStream.Models;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.App.CensusStream.EventProcessors
{
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
            var dataModel = new Data.Models.Planetside.Events.VehicleDestroy
            {
                AttackerCharacterId = payload.AttackerCharacterId,
                AttackerLoadoutId = payload.AttackerLoadoutId,
                AttackerVehicleId = payload.AttackerVehicleId,
                AttackerWeaponId = payload.AttackerWeaponId,
                CharacterId = payload.CharacterId,
                VehicleId = payload.VehicleId,
                FactionId = payload.FactionId,
                FacilityId = payload.FacilityId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };

            await _eventRepository.AddAsync(dataModel);
        }
    }
}
