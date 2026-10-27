using AsyncKeyedLock;
using DaybreakGames.Census.Exceptions;
using Microsoft.Extensions.Logging;
using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.Census.Collection;
using Voidwell.DaybreakGames.Census.Models;
using Voidwell.DaybreakGames.Census.Platforms;
using Voidwell.DaybreakGames.CensusStore.Platforms;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Data.Repositories.Abstractions;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.CensusStore.Services;

public class OutfitStore : IOutfitStore
{
    private readonly IOutfitRepository _outfitRepository;
    private readonly OutfitCollection _outfitCollection;
    private readonly OutfitMembershipCollection _outfitMembershipCollection;
    private readonly CharacterCollection _characterCollection;
    private readonly ICache _cache;
    private readonly ILogger<OutfitStore> _logger;
    private readonly IPlatformResolver _platformResolver;
    private readonly ICensusContext _censusContext;

    private const string _cacheKey = "ps2.outfitstore";
    private readonly Func<long, string> _getOutfitCacheKey = outfitId => $"{_cacheKey}_outfit_{outfitId}";
    private readonly Func<long, string> _getMemberCacheKey = outfitId => $"{_cacheKey}_member_{outfitId}";
    private readonly Func<string, Platform?, string> _getAliasIdCacheKey = (outfitAlias, platform) => $"{_cacheKey}_name_{platform}_{outfitAlias}";

    private readonly TimeSpan _cacheOutfitExpiration = TimeSpan.FromMinutes(15);
    private readonly TimeSpan _cacheOutfitNameExpiration = TimeSpan.FromMinutes(30);
    private readonly TimeSpan _cacheOutfitMemberExpiration = TimeSpan.FromMinutes(10);
    private readonly TimeSpan _cacheOutfitMemberDetailsExpiration = TimeSpan.FromMinutes(30);

    private readonly AsyncKeyedLocker<long> _outfitLock = new();
    private readonly AsyncKeyedLocker<long> _outfitMembershipLock = new();

    public OutfitStore(IOutfitRepository outfitRepository, OutfitCollection outfitCollection,
        OutfitMembershipCollection outfitMembershipCollection, CharacterCollection characterCollection, ICache cache,
        ILogger<OutfitStore> logger, IPlatformResolver platformResolver, ICensusContext censusContext)
    {
        _outfitRepository = outfitRepository;
        _outfitCollection = outfitCollection;
        _outfitMembershipCollection = outfitMembershipCollection;
        _characterCollection = characterCollection;
        _cache = cache;
        _logger = logger;
        _platformResolver = platformResolver;
        _censusContext = censusContext;
    }

    public Task<IEnumerable<Outfit>> GetOutfitsByIdsAsync(IEnumerable<long> outfitIds)
    {
        return _outfitRepository.GetOutfitsByIdsAsync(outfitIds);
    }

    public async Task<Outfit?> GetOutfitDetailsAsync(long outfitId)
    {
        var outfit = await _outfitRepository.GetOutfitDetailsAsync(outfitId);

        if (outfit != null)
        {
            return outfit;
        }

        await GetOutfitAsync(outfitId);

        return await _outfitRepository.GetOutfitDetailsAsync(outfitId);
    }

    public async Task<long?> GetOutfitIdByAliasAsync(string outfitAlias)
    {
        return await _cache.GetOrSetIfNotNullAsync<long?>(
            _getAliasIdCacheKey(outfitAlias, _censusContext.Platform),
            async ct => await _outfitRepository.GetOutfitIdByAlias(outfitAlias, await _platformResolver.GetContextWorldIdsAsync()),
            _cacheOutfitNameExpiration);
    }

    public async Task<IEnumerable<OutfitMember>> GetOutfitMembersAsync(long outfitId)
    {
        return await _cache.GetOrSetAsync<IEnumerable<OutfitMember>>(_getMemberCacheKey(outfitId), ct => _outfitRepository.GetOutfitMembersAsync(outfitId), _cacheOutfitMemberDetailsExpiration);
    }

    public async Task<IEnumerable<Outfit>> GetOutfitsByNameAsync(string name, int limit = 12)
    {
        return await _outfitRepository.GetOutfitsByNameAsync(name, limit, await _platformResolver.GetContextWorldIdsAsync());
    }

    public async Task<Outfit> GetOutfitByAliasAsync(string alias)
    {
        return (await _outfitRepository.GetOutfitByAliasAsync(alias, await _platformResolver.GetContextWorldIdsAsync()))!;
    }

    public async Task<OutfitMember?> UpdateCharacterOutfitMembershipAsync(Character character)
    {
        OutfitMember outfitMember;

        using (await _outfitMembershipLock.LockAsync(character.Id))
        {
            var cacheKey = $"{_cacheKey}_member_{character.Id}";

            outfitMember = await _cache.GetAsync<OutfitMember>(cacheKey);
            if (outfitMember != null)
            {
                return outfitMember.OutfitId != 0 ? outfitMember : null;
            }

            CensusOutfitMemberModel membership;

            var platform = await _platformResolver.GetWorldPlatformAsync(character.WorldId);
            using var platformScope = _censusContext.Use(platform);

            try
            {
                membership = (await _outfitMembershipCollection.GetCharacterOutfitMembershipAsync(character.Id))!;
            }
            catch (CensusConnectionException)
            {
                return null;
            }

            if (membership == null)
            {
                await _outfitRepository.RemoveOutfitMemberAsync(character.Id);
                await _cache.SetAsync(cacheKey, new OutfitMember(), _cacheOutfitMemberExpiration);
                return null;
            }

            var outfit = await GetLatestOutfitAsync(membership.OutfitId, character);
            if (outfit == null)
            {
                _logger.LogError(84624, "Unable to resolve outfit {OutfitId} for character {CharacterId}", membership.OutfitId, character.Id);
                await _cache.SetAsync(cacheKey, new OutfitMember(), _cacheOutfitMemberExpiration);
                return null;
            }

            outfitMember = new OutfitMember
            {
                OutfitId = membership.OutfitId,
                CharacterId = membership.CharacterId,
                MemberSinceDate = membership.MemberSinceDate,
                Rank = membership.Rank,
                RankOrdinal = membership.RankOrdinal
            };

            await _cache.SetAsync(cacheKey, outfitMember, _cacheOutfitMemberExpiration);
            outfitMember = await _outfitRepository.UpsertAsync(outfitMember);
        }

        return outfitMember;
    }

    public async Task<Outfit?> GetOutfitAsync(long outfitId, Character? member = null)
    {
        Outfit outfit;

        using (await _outfitLock.LockAsync(outfitId))
        {
            outfit = (await GetKnownOutfitAsync(outfitId))!;
            if (outfit == null)
            {
                return null;
            }

            if (outfit.WorldId == null || outfit.FactionId == null)
            {
                outfit = await ResolveOutfitDetailsAsync(outfit, member);
                await _outfitRepository.UpsertAsync(outfit);
            }

            await _cache.SetAsync(_getOutfitCacheKey(outfitId), outfit, _cacheOutfitExpiration);
        }

        return outfit;
    }

    private async Task<Outfit?> GetKnownOutfitAsync(long outfitId)
    {

        var outfit = await _cache.GetAsync<Outfit>(_getOutfitCacheKey(outfitId));
        if (outfit != null)
        {
            return outfit;
        }

        outfit = await _outfitRepository.GetOutfitAsync(outfitId);
        if (outfit == null)
        {
            try
            {
                outfit = await GetCensusOutfitAsync(outfitId);
            }
            catch (CensusConnectionException)
            {
                return null;
            }
        }

        return outfit;
    }

    private async Task<Outfit?> GetCensusOutfitAsync(long outfitId)
    {
        var platform = await _platformResolver.GetOutfitPlatformAsync(outfitId);
        using var platformScope = _censusContext.Use(platform);

        var censusOutfit = await _outfitCollection.GetOutfitAsync(outfitId);
        if (censusOutfit == null)
        {
            return null;
        }

        return new Outfit
        {
            Id = censusOutfit.OutfitId,
            Alias = censusOutfit.Alias,
            Name = censusOutfit.Name,
            LeaderCharacterId = censusOutfit.LeaderCharacterId,
            CreatedDate = censusOutfit.TimeCreated,
            MemberCount = censusOutfit.MemberCount
        };
    }

    private async Task<Outfit?> GetLatestOutfitAsync(long outfitId, Character character)
    {
        Outfit outfit;

        using (await _outfitLock.LockAsync(outfitId))
        {
            outfit = await _cache.GetAsync<Outfit>(_getOutfitCacheKey(outfitId));
            if (outfit != null)
            {
                return outfit;
            }

            outfit = (await GetCensusOutfitAsync(outfitId))!;
            if (outfit == null)
            {
                return null;
            }

            outfit = await ResolveOutfitDetailsAsync(outfit, character);

            await _outfitRepository.UpsertAsync(outfit);
            await _cache.SetAsync(_getOutfitCacheKey(outfitId), outfit, _cacheOutfitExpiration);
        }

        return outfit;
    }

    private async Task<Outfit> ResolveOutfitDetailsAsync(Outfit outfit, Character? member = null)
    {
        if (member != null)
        {
            outfit.WorldId = member.WorldId;
            outfit.FactionId = member.FactionId;
        }
        else if (outfit.LeaderCharacterId != null)
        {
            var platform = await _platformResolver.GetCharacterPlatformAsync(outfit.LeaderCharacterId.Value);
            using var platformScope = _censusContext.Use(platform);

            var leader = await _characterCollection.GetCharacterAsync(outfit.LeaderCharacterId.Value);
            outfit.WorldId = leader!.WorldId;
            outfit.FactionId = leader.FactionId;
        }

        return outfit;
    }
}
