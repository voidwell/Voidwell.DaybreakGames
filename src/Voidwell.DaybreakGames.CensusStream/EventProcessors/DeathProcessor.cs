using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.CensusStream;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.CensusStream.EventProcessors
{
    [CensusEventProcessor("Death")]
    public class DeathProcessor: IEventProcessor<Death>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICharacterService _characterService;
        private readonly ICharacterRatingService _characterRatingService;
        private readonly IPlayerMonitor _playerMonitor;

        public DeathProcessor(IEventRepository eventRepository, ICharacterService characterService,
            ICharacterRatingService characterRatingService, IPlayerMonitor playerMonitor)
        {
            _eventRepository = eventRepository;
            _characterService = characterService;
            _characterRatingService = characterRatingService;
            _playerMonitor = playerMonitor;
        }

        public async Task Process(Death payload)
        {
            var taskList = new List<Task>();
            Task<OutfitMember> attackerOutfitTask = null;
            Task<OutfitMember> victimOutfitTask = null;

            taskList.Add(SetLastSeenAsync(payload));

            if (payload.AttackerCharacterId != null && payload.AttackerCharacterId.Length > 18)
            {
                attackerOutfitTask = _characterService.GetCharactersOutfit(payload.AttackerCharacterId);
                taskList.Add(attackerOutfitTask);
            }

            if (payload.CharacterId != null && payload.CharacterId.Length > 18)
            {
                victimOutfitTask = _characterService.GetCharactersOutfit(payload.CharacterId);
                taskList.Add(victimOutfitTask);
            }

            if (payload.AttackerCharacterId != null && payload.CharacterId != null &&
                payload.AttackerCharacterId != payload.CharacterId &&
                payload.AttackerCharacterId.Length > 18 && payload.CharacterId.Length > 18)
            {
                taskList.Add(_characterRatingService.CalculateRatingAsync(payload.AttackerCharacterId, payload.CharacterId));
            }

            await Task.WhenAll(taskList);

            var dataModel = new Data.Models.Planetside.Events.Death
            {
                AttackerCharacterId = payload.AttackerCharacterId,
                AttackerFireModeId = payload.AttackerFireModeId,
                AttackerLoadoutId = payload.AttackerLoadoutId,
                AttackerVehicleId = payload.AttackerVehicleId,
                AttackerWeaponId = payload.AttackerWeaponId,
                AttackerOutfitId = attackerOutfitTask?.Result?.OutfitId,
                CharacterId = payload.CharacterId,
                CharacterLoadoutId = payload.CharacterLoadoutId,
                CharacterOutfitId = victimOutfitTask?.Result?.OutfitId,
                IsHeadshot = payload.IsHeadshot,
                Timestamp = payload.Timestamp,
                WorldId = payload.WorldId,
                ZoneId = payload.ZoneId.Value
            };

            await _eventRepository.AddAsync(dataModel);
        }

        private Task SetLastSeenAsync(Death payload)
        {
            return Task.WhenAll(
                _playerMonitor.SetLastSeenAsync(payload.AttackerCharacterId, payload.ZoneId.Value, payload.Timestamp),
                _playerMonitor.SetLastSeenAsync(payload.CharacterId, payload.ZoneId.Value, payload.Timestamp));
        }
    }
}
