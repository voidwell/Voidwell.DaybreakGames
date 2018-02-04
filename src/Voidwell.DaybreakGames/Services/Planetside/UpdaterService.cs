using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class UpdaterService : IUpdaterService
    {
        private readonly ICharacterUpdaterRepository _characterUpdaterRepository;

        public bool IsRunning { get; private set; }

        public UpdaterService(ICharacterUpdaterRepository characterUpdaterRepository)
        {
            _characterUpdaterRepository = characterUpdaterRepository;

            IsRunning = false;
        }

        public async Task AddToQueue(string characterId)
        {
            var dataModel = new CharacterUpdateQueue
            {
                CharacterId = characterId,
                Timestamp = DateTime.UtcNow
            };
            await _characterUpdaterRepository.AddAsync(dataModel);
        }

        public void StartUpdater()
        {
            IsRunning = true;
            CharacterQueueUpdater();
        }

        public void StopUpdater()
        {
            IsRunning = false;
        }

        private void CharacterQueueUpdater()
        {

        }
    }
}
