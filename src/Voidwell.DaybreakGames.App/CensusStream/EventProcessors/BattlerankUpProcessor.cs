using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStream;
using Voidwell.DaybreakGames.CensusStream.Models;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.App.CensusStream.EventProcessors
{
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
            var dataModel = new Data.Models.Planetside.Events.BattlerankUp
            {
                BattleRank = payload.BattleRank,
                CharacterId = payload.CharacterId,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };
            await Task.WhenAll(_eventRepository.AddAsync(dataModel), _playerMonitor.SetLastSeenAsync(dataModel.CharacterId, dataModel.ZoneId, dataModel.Timestamp));
        }
    }
}
