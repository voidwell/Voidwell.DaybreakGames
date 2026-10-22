using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Live.CensusStream.Models;
using Voidwell.DaybreakGames.Live.GameState;
using Voidwell.DaybreakGames.Live.Mappers;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Live.CensusStream.EventProcessors;

[CensusEventProcessor("Death")]
public class DeathProcessor : IEventProcessor<Death>
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
        var attackerOutfitIdTask = GetCharacterOutfitIdAsync(payload.AttackerCharacterId);
        var victimOutfitIdTask = GetCharacterOutfitIdAsync(payload.CharacterId);

        await Task.WhenAll(
            SetLastSeenAsync(payload),
            //CalculateCharacterRatingsAsync(payload),
            attackerOutfitIdTask,
            victimOutfitIdTask
        );

        var dataModel = CensusToDataMapper.Map(payload);
        dataModel.AttackerOutfitId = attackerOutfitIdTask?.Result;
        dataModel.CharacterOutfitId = victimOutfitIdTask?.Result;

        await _eventRepository.AddAsync(dataModel);
    }

    private Task SetLastSeenAsync(Death payload)
    {
        return Task.WhenAll(
            _playerMonitor.SetLastSeenAsync(payload.AttackerCharacterId, payload.ZoneId!.Value, payload.Timestamp),
            _playerMonitor.SetLastSeenAsync(payload.CharacterId, payload.ZoneId.Value, payload.Timestamp));
    }

    // Player character ids are 19 digits; shorter ids (such as 0) are not players
    private static bool IsValidCharacterId(long characterId)
    {
        return characterId >= 1_000_000_000_000_000_000;
    }

    private async Task<long?> GetCharacterOutfitIdAsync(long characterId)
    {
        if (!IsValidCharacterId(characterId))
        {
            return null;
        }

        var outfit = await _characterService.GetCharactersOutfit(characterId);
        return outfit?.OutfitId;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051", Justification = "Currently disabled in Process; retained for re-enabling")]
    private Task CalculateCharacterRatingsAsync(Death payload)
    {
        if (!IsValidCharacterId(payload.AttackerCharacterId!) ||
            !IsValidCharacterId(payload.CharacterId!) ||
            payload.AttackerCharacterId == payload.CharacterId)
        {
            return Task.CompletedTask;
        }

        return _characterRatingService.CalculateRatingAsync(payload.AttackerCharacterId!, payload.CharacterId!);
    }
}
