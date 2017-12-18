using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class UpdaterService : IUpdaterService
    {
        private readonly Func<PS2DbContext> _ps2DbContextFactory;

        public bool IsRunning { get; private set; }

        public UpdaterService(Func<PS2DbContext> ps2DbContextFactory)
        {
            _ps2DbContextFactory = ps2DbContextFactory;

            IsRunning = false;
        }

        public async Task AddToQueue(string characterId)
        {
            var dbContext = _ps2DbContextFactory();
            var dataModel = new DbCharacterUpdateQueue
            {
                CharacterId = characterId,
                Timestamp = DateTime.UtcNow
            };

            dbContext.CharacterUpdateQueue.Update(dataModel);
            await dbContext.SaveChangesAsync();
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
