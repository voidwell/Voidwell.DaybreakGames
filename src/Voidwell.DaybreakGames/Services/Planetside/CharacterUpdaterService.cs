using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class CharacterUpdaterService : StatefulHostedService, ICharacterUpdaterService
    {
        private readonly ICharacterUpdaterRepository _characterUpdaterRepository;
        private readonly ICharacterService _characterService;
        private readonly DaybreakGamesOptions _options;
        private readonly ILogger _logger;

        public override string ServiceName => "CharacterUpdater";

        private Timer _timer;
        private readonly TimeSpan _executionInterval = TimeSpan.FromSeconds(10);
        private const int _maxParallelUpdates = 2;
        private bool _isWorking;
        private SemaphoreSlim _parallelSemaphore;

        public CharacterUpdaterService(ICharacterUpdaterRepository characterUpdaterRepository, ICharacterService characterService,
            IOptions<DaybreakGamesOptions> options, ICache cache, ILogger<CharacterUpdaterService> logger)
            : base(cache)
        {
            _characterUpdaterRepository = characterUpdaterRepository;
            _characterService = characterService;
            _options = options.Value;
            _logger = logger;

            _isWorking = false;
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

        public override Task StartInternalAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();

            if (_options.DisableCharacterUpdater)
            {
                _logger.LogInformation("Character updater is disabled");
                return Task.CompletedTask;
            }

            _logger.LogInformation("Character updater started");

            _timer = new Timer(ExecuteAsync, null, 0, (int)_executionInterval.TotalMilliseconds);

            return Task.CompletedTask;
        }

        public override Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();

            _logger.LogInformation("Character updater stopped");

            return Task.CompletedTask;
        }

        public override async Task<ServiceState> GetStatus(CancellationToken cancellationToken)
        {
            var status = await base.GetStatus(cancellationToken);

            int count = await _characterUpdaterRepository.GetQueueLengthAsync();
            status.Details = new { QueueLength = count };

            return status;
        }

        private async void ExecuteAsync(Object stateInfo)
        {
            if (_isWorking)
                return;

            _isWorking = true;
            _parallelSemaphore = new SemaphoreSlim(_maxParallelUpdates);

            var characterQueue = await _characterUpdaterRepository.GetAllAsync();
            await Task.WhenAll(characterQueue.Select(UpdateCharacter));

            _isWorking = false;
        }

        private async Task UpdateCharacter(CharacterUpdateQueue characterItem)
        {
            await _parallelSemaphore.WaitAsync();

            if (!_isRunning)
                return;

            var character = await _characterService.GetCharacter(characterItem.CharacterId);
            try
            {
                await _characterService.UpdateAllCharacterInfo(characterItem.CharacterId, character?.Time?.LastSaveDate);
                await _characterUpdaterRepository.RemoveAsync(characterItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update for character {characterItem.CharacterId}");
            }

            _parallelSemaphore.Release();
        }
    }
}
