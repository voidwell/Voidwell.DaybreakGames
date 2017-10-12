using System;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.DBContext;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class UpdaterService : IUpdaterService, IDisposable
    {
        private readonly PS2DbContext _ps2DbContext;
        private bool _isRunning;

        public UpdaterService(PS2DbContext ps2DbContext)
        {
            _ps2DbContext = ps2DbContext;

            _isRunning = false;
        }

        public async Task AddToQueue(string characterId)
        {
            var dataModel = new DbCharacterUpdateQueue
            {
                CharacterId = characterId,
                Timestamp = DateTime.UtcNow
            };

            _ps2DbContext.CharacterUpdateQueue.Update(dataModel);
            await _ps2DbContext.SaveChangesAsync();
        }

        public void StartUpdater()
        {
            _isRunning = true;
            CharacterQueueUpdater();
        }

        public void StopUpdater()
        {
            _isRunning = false;
        }

        private void CharacterQueueUpdater()
        {

        }

        public void Dispose()
        {
            _ps2DbContext?.Dispose();
        }
    }
}
