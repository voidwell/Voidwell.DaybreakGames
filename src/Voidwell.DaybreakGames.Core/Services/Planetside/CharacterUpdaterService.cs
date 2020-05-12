using System;
using Microsoft.Extensions.Logging;
using Voidwell.DaybreakGames.Data.Repositories;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public class CharacterUpdaterService : ICharacterUpdaterService
    {
        private readonly ICharacterUpdaterRepository _characterUpdaterRepository;
        private readonly ILogger<CharacterUpdaterService> _logger;

        public CharacterUpdaterService(ICharacterUpdaterRepository characterUpdaterRepository, ILogger<CharacterUpdaterService> logger)
        {
            _characterUpdaterRepository = characterUpdaterRepository;
            _logger = logger;
        }

        public async Task AddToQueue(string characterId)
        {
            try
            {
                var dataModel = new CharacterUpdateQueue
                {
                    CharacterId = characterId,
                    Timestamp = DateTime.UtcNow
                };
                await _characterUpdaterRepository.AddAsync(dataModel);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add character '{characterId}' to updater", characterId);
            }
        }

        public Task<IEnumerable<CharacterUpdateQueue>> GetAllAsync(TimeSpan delay)
        {
            return _characterUpdaterRepository.GetAllAsync(delay);
        }

        public Task<int> GetQueueLengthAsync()
        {
            return _characterUpdaterRepository.GetQueueLengthAsync();
        }

        public Task RemoveAsync(CharacterUpdateQueue queueItem)
        {
            return _characterUpdaterRepository.RemoveAsync(queueItem);
        }
    }
}
