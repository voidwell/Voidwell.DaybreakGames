using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.CensusStore.Services.Abstractions;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside;

public class OutfitService : IOutfitService
{
    private readonly IOutfitStore _outfitStore;
    private readonly ICache _cache;

    private const string _cacheKey = "ps2.outfit";
    private readonly Func<string, string> _getDetailsCacheKey = outfitId => $"{_cacheKey}_details_{outfitId}";

    private readonly TimeSpan _cacheOutfitDetailsExpiration = TimeSpan.FromMinutes(30);

    public OutfitService(IOutfitStore outfitStore, ICache cache)
    {
        _outfitStore = outfitStore;
        _cache = cache;
    }

    public Task<IEnumerable<Outfit>> FindOutfits(IEnumerable<string> outfitIds)
    {
        return _outfitStore.GetOutfitsByIdsAsync(outfitIds);
    }

    public async Task<OutfitDetails?> GetOutfitDetails(string outfitId)
    {
        return await _cache.GetOrSetIfNotNullAsync<OutfitDetails>(_getDetailsCacheKey(outfitId), async ct =>
        {
            var outfit = await _outfitStore.GetOutfitDetailsAsync(outfitId);
            if (outfit == null)
            {
                return null;
            }

            var outfitMembers = await GetOutfitMembers(outfitId);

            var details = new OutfitDetails
            {
                OutfitId = outfit.Id,
                Name = outfit.Name,
                Alias = outfit.Alias,
                CreatedDate = outfit.CreatedDate,
                FactionId = outfit.FactionId,
                FactionName = outfit.Faction?.Name,
                FactionImageId = outfit.Faction?.ImageId,
                MemberCount = outfit.MemberCount,
                WorldId = outfit.WorldId,
                WorldName = outfit.World?.Name,
                LeaderCharacterId = outfit.LeaderCharacterId,
                LeaderName = outfit.LeaderCharacter?.Name,
                TrackedMemberCount = outfitMembers.Count(),
                Activity7Days = outfitMembers.Count(a => DateTime.UtcNow - a.LastLoginDate.GetValueOrDefault() <= TimeSpan.FromDays(7)),
                Activity30Days = outfitMembers.Count(a => DateTime.UtcNow - a.LastLoginDate.GetValueOrDefault() <= TimeSpan.FromDays(30)),
                Activity90Days = outfitMembers.Count(a => DateTime.UtcNow - a.LastLoginDate.GetValueOrDefault() <= TimeSpan.FromDays(90))
            };

            return details;
        }, _cacheOutfitDetailsExpiration);
    }

    public async Task<OutfitDetails?> GetOutfitByAlias(string outfitAlias)
    {
        var outfitId = await _outfitStore.GetOutfitIdByAliasAsync(outfitAlias);
        if (outfitId == null)
        {
            return null;
        }

        return await GetOutfitDetails(outfitId);
    }

    public async Task<IEnumerable<OutfitMemberDetails>> GetOutfitMembers(string outfitId)
    {
        var members = await _outfitStore.GetOutfitMembersAsync(outfitId);

        return members.Where(a => a.Character!.Time != null).Select(a => new OutfitMemberDetails
        {
            CharacterId = a.CharacterId,
            MemberSinceDate = a.MemberSinceDate!.Value,
            Rank = a.Rank,
            RankOrdinal = a.RankOrdinal!.Value,
            Name = a.Character?.Name,
            BattleRank = a.Character?.BattleRank,
            LastLoginDate = a.Character?.Time?.LastLoginDate,
            LifetimeStats = new OutfitMemberDetailsStats
            {
                FacilityCaptureCount = a.Character!.LifetimeStats!.FacilityCaptureCount,
                FacilityDefendedCount = a.Character.LifetimeStats.FacilityDefendedCount,
                WeaponKills = a.Character.LifetimeStats.WeaponKills,
                WeaponDeaths = a.Character.LifetimeStats.WeaponDeaths,
                WeaponFireCount = a.Character.LifetimeStats.WeaponFireCount,
                WeaponHeadshots = a.Character.LifetimeStats.WeaponHeadshots,
                WeaponHitCount = a.Character.LifetimeStats.WeaponHitCount,
                WeaponPlayTime = a.Character.LifetimeStats.WeaponPlayTime,
                WeaponScore = a.Character.LifetimeStats.WeaponScore,
                WeaponVehicleKills = a.Character.LifetimeStats.WeaponVehicleKills,
                AssistCount = a.Character.LifetimeStats.AssistCount,
                RevengeCount = a.Character.LifetimeStats.RevengeCount,
                DominationCount = a.Character.LifetimeStats.DominationCount
            }
        });
    }

    public Task<IEnumerable<Outfit>> LookupOutfitsByName(string name, int limit = 12)
    {
        return _outfitStore.GetOutfitsByNameAsync(name, limit);
    }

    public Task<Outfit> LookupOutfitByAlias(string alias)
    {
        return _outfitStore.GetOutfitByAliasAsync(alias);
    }
}
