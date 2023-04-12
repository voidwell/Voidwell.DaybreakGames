using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DaybreakGames.Census.Exceptions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Utils.HostedService;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;

namespace Voidwell.DaybreakGames.Live
{
    public class CharacterUpdaterService : ICharacterUpdaterService
    {
        private readonly ICharacterUpdaterRepository _characterUpdaterRepository;
        private readonly ICharacterService _characterService;
        private readonly ICharacterDirectiveService _characterDirectiveService;
        private readonly HostedServiceState<ICharacterUpdaterService> _state;
        private readonly LiveOptions _options;
        private readonly ILogger _logger;

        private Timer _timer;
        private readonly TimeSpan _executionInterval = TimeSpan.FromSeconds(10);
        private readonly TimeSpan _updateDelay = TimeSpan.FromHours(1);
        private const int _maxParallelUpdates = 1;
        private bool _isWorking;
        private bool _waitError;
        private SemaphoreSlim _parallelSemaphore;

        public CharacterUpdaterService(ICharacterUpdaterRepository characterUpdaterRepository, ICharacterService characterService,
            ICharacterDirectiveService characterDirectiveService, HostedServiceState<ICharacterUpdaterService> state,
            IOptions<LiveOptions> options, ILogger<CharacterUpdaterService> logger)
        {
            _characterUpdaterRepository = characterUpdaterRepository;
            _characterService = characterService;
            _characterDirectiveService = characterDirectiveService;
            _state = state;
            _options = options.Value;
            _logger = logger;

            _isWorking = false;
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
            catch(Exception ex)
            {
                _logger.LogWarning(ex, "Failed to add character '{characterId}' to updater", characterId);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();

            _logger.LogInformation("Character updater stopped");

            return Task.CompletedTask;
        }

        public async Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            var totalCountTask = _characterUpdaterRepository.GetQueueLengthAsync();
            var activeCountTask = _characterUpdaterRepository.GetQueueLengthAsync(_updateDelay);

            await Task.WhenAll(totalCountTask, activeCountTask);

            return new { ActiveQueue = activeCountTask.Result, QueueLength = totalCountTask.Result };
        }

        private async void ExecuteAsync(object stateInfo)
        {
            if (_isWorking)
                return;

            _isWorking = true;
            _waitError = false;
            _parallelSemaphore = new SemaphoreSlim(_maxParallelUpdates);

            var characterQueue = await _characterUpdaterRepository.GetAllAsync(_updateDelay);
            await Task.WhenAll(characterQueue.Select(UpdateCharacter));
            _isWorking = false;
        }

        private async Task UpdateCharacter(CharacterUpdateQueue characterItem)
        {
            await _parallelSemaphore.WaitAsync();

            try
            {
                if (!_state.IsRunning || _waitError)
                    return;

                var character = await _characterService.GetCharacter(characterItem.CharacterId);
                try
                {
                    var lastSaveDate = character?.Time?.LastSaveDate;
                    lastSaveDate = lastSaveDate?.AddHours(-12);

                    await Task.WhenAll(
                        _characterService.UpdateAllCharacterInfo(characterItem.CharacterId, lastSaveDate),
                        _characterDirectiveService.UpdateCharacterDirectivesAsync(characterItem.CharacterId));

                    await _characterUpdaterRepository.RemoveAsync(characterItem);
                }
                catch (CensusServiceUnavailableException)
                {
                    _logger.LogError(75214, $"Service Unavailable when trying to update character {characterItem.CharacterId}");
                    _waitError = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to update for character {characterItem.CharacterId}");
                }
            }
            finally
            {
                _parallelSemaphore.Release();
            }
        }

        public Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
