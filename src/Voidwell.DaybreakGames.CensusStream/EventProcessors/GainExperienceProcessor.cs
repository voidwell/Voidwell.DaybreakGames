using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.CensusStream.EventProcessors
{
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
            var dataModel = new Data.Models.Planetside.Events.GainExperience
            {
                Id = Guid.NewGuid(),
                ExperienceId = payload.ExperienceId,
                CharacterId = payload.CharacterId,
                Amount = payload.Amount,
                LoadoutId = payload.LoadoutId,
                OtherId = payload.OtherId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };

            await Task.WhenAll(_eventRepository.AddAsync(dataModel), _playerMonitor.SetLastSeenAsync(dataModel.CharacterId, dataModel.ZoneId, dataModel.Timestamp));
        }
    }
}
