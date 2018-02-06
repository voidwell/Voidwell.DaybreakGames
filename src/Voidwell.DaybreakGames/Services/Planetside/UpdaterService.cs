using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class UpdaterService : IUpdaterService
    {
        private readonly ICharacterUpdaterRepository _characterUpdaterRepository;
        private readonly ICharacterService _characterService;
        private readonly DaybreakGamesOptions _options;
        private readonly ILogger _logger;

        public bool IsRunning { get; private set; }

        private Timer _timer;
        private readonly TimeSpan _executionInterval = TimeSpan.FromSeconds(10);
        private bool _isWorking;

        public UpdaterService(ICharacterUpdaterRepository characterUpdaterRepository, ICharacterService characterService, IOptions<DaybreakGamesOptions> options, ILogger<UpdaterService> logger)
        {
            _characterUpdaterRepository = characterUpdaterRepository;
            _characterService = characterService;
            _options = options.Value;
            _logger = logger;

            _isWorking = false;

            IsRunning = !_options.DisableCharacterUpdater;
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

        public Task StartUpdater()
        {
            IsRunning = true;
            SetupTimer();

            return Task.CompletedTask;
        }

        public Task StopUpdater()
        {
            _timer.Dispose();
            IsRunning = false;

            _logger.LogInformation("Character updater stopped");

            return Task.CompletedTask;
        }

        public Task Startup()
        {
            if (!IsRunning)
                return Task.CompletedTask;

            SetupTimer();

            return Task.CompletedTask;
        }

        private void SetupTimer()
        {
            if (_timer != null)
                _timer.Dispose();

            _logger.LogInformation("Character updater started");

            _timer = new Timer(ExecuteAsync, null, 0, (int)_executionInterval.TotalMilliseconds);
        }

        private async void ExecuteAsync(Object stateInfo)
        {
            if (_isWorking)
                return;

            _isWorking = true;

            var characterQueue = await _characterUpdaterRepository.GetAllAsync();

            foreach (var item in characterQueue)
            {
                var character = await _characterService.GetCharacterDetails(item.CharacterId);
                try
                {
                    await _characterService.UpdateAllCharacterInfo(item.CharacterId, character?.Times?.LastSaveDate);
                    await _characterUpdaterRepository.RemoveAsync(item);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to update for character {item.CharacterId}");
                }
            }

            _isWorking = false;
        }
    }
}
