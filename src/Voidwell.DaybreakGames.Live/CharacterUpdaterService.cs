using DaybreakGames.Census.Exceptions;
using Microsoft.Extensions.Logging;
using Voidwell.Common.State;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Live;

public class CharacterUpdaterService : StatefulHostedService, ICharacterUpdaterService
{
    private readonly ICharacterUpdaterRepository _characterUpdaterRepository;
    private readonly ICharacterService _characterService;
    private readonly ICharacterDirectiveService _characterDirectiveService;
    private readonly ILogger _logger;

    private Timer? _timer;
    private readonly TimeSpan _executionInterval = TimeSpan.FromSeconds(10);
    private readonly TimeSpan _updateDelay = TimeSpan.FromHours(1);
    private const int _maxParallelUpdates = 1;
    private bool _isWorking;
    private volatile bool _isRunning;
    private bool _waitError;
    private SemaphoreSlim? _parallelSemaphore;

    public CharacterUpdaterService(ICharacterUpdaterRepository characterUpdaterRepository, ICharacterService characterService,
        ICharacterDirectiveService characterDirectiveService, ILogger<CharacterUpdaterService> logger)
    {
        _characterUpdaterRepository = characterUpdaterRepository;
        _characterService = characterService;
        _characterDirectiveService = characterDirectiveService;
        _logger = logger;

        _isWorking = false;
    }

    public async Task AddToQueue(long characterId)
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

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        _isRunning = true;

        _logger.LogInformation("Character updater started");

        _timer = new Timer(ExecuteAsync, null, 0, (int)_executionInterval.TotalMilliseconds);

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        // Stops queued updates from starting; one already in progress finishes
        _isRunning = false;
        _timer?.Dispose();

        _logger.LogInformation("Character updater stopped");

        return Task.CompletedTask;
    }

    public override async Task<object?> GetStatusAsync(CancellationToken cancellationToken)
    {
        var totalCountTask = _characterUpdaterRepository.GetQueueLengthAsync();
        var activeCountTask = _characterUpdaterRepository.GetQueueLengthAsync(_updateDelay);

        await Task.WhenAll(totalCountTask, activeCountTask);

        return new { ActiveQueue = activeCountTask.Result, QueueLength = totalCountTask.Result };
    }

    private async void ExecuteAsync(object? stateInfo)
    {
        if (_isWorking)
        {
            return;
        }

        _isWorking = true;
        _waitError = false;
        _parallelSemaphore = new SemaphoreSlim(_maxParallelUpdates);

        var characterQueue = await _characterUpdaterRepository.GetAllAsync(_updateDelay);
        await Task.WhenAll(characterQueue.Select(UpdateCharacterAsync));
        _isWorking = false;
    }

    private async Task UpdateCharacterAsync(CharacterUpdateQueue characterItem)
    {
        await _parallelSemaphore!.WaitAsync();

        try
        {
            if (!_isRunning || _waitError)
            {
                return;
            }

            var character = await _characterService.GetCharacter(characterItem.CharacterId!);
            try
            {
                var lastSaveDate = character?.Time?.LastSaveDate;
                lastSaveDate = lastSaveDate?.AddHours(-12);

                await Task.WhenAll(
                    _characterService.UpdateAllCharacterInfo(characterItem.CharacterId!, lastSaveDate),
                    _characterDirectiveService.UpdateCharacterDirectivesAsync(characterItem.CharacterId!));

                await _characterUpdaterRepository.RemoveAsync(characterItem);
            }
            catch (CensusServiceUnavailableException)
            {
                _logger.LogError(75214, "Service Unavailable when trying to update character {CharacterId}", characterItem.CharacterId);
                _waitError = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update for character {CharacterId}", characterItem.CharacterId);
            }
        }
        finally
        {
            _parallelSemaphore.Release();
        }
    }
}
