using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories;

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
        private bool _isWorking;

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
