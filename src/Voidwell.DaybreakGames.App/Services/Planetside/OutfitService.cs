using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;
using Voidwell.DaybreakGames.CensusServices;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Data.Repositories;
using Microsoft.Extensions.Logging;
using Voidwell.DaybreakGames.Models;
using Voidwell.Cache;
using System;
using System.Linq;
using DaybreakGames.Census.Exceptions;
using Voidwell.DaybreakGames.CensusServices.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class OutfitService : IOutfitService
    {
        private readonly IOutfitRepository _outfitRepository;
        private readonly CensusOutfit _censusOutfit;
        private readonly CensusCharacter _censusCharacter;
        private readonly ICache _cache;
        private readonly ILogger<OutfitService> _logger;

        private readonly string _cacheKey = "ps2.outfit";
        private readonly TimeSpan _cacheOutfitExpiration = TimeSpan.FromMinutes(15);
        private readonly TimeSpan _cacheOutfitNameExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _cacheOutfitMemberExpiration = TimeSpan.FromMinutes(10);
        private readonly TimeSpan _cacheOutfitDetailsExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _cacheOutfitMemberDetailsExpiration = TimeSpan.FromMinutes(30);


        private readonly KeyedSemaphoreSlim _outfitLock = new KeyedSemaphoreSlim();
        private readonly KeyedSemaphoreSlim _outfitMembershipLock = new KeyedSemaphoreSlim();

        public OutfitService(IOutfitRepository outfitRepository, CensusOutfit censusOutfit, CensusCharacter censusCharacter, ICache cache, ILogger<OutfitService> logger)
        {
            _outfitRepository = outfitRepository;
            _censusOutfit = censusOutfit;
            _censusCharacter = censusCharacter;
            _cache = cache;
            _logger = logger;
        }

        public Task<IEnumerable<Outfit>> FindOutfits(IEnumerable<string> outfitIds)
        {
            return _outfitRepository.GetOutfitsByIdsAsync(outfitIds);
        }

        public Task<Outfit> GetOutfit(string outfitId)
        {
            return GetOutfitAsync(outfitId);
        }

        public async Task<OutfitDetails> GetOutfitDetails(string outfitId)
        {
            var cacheKey = $"{_cacheKey}_details_{outfitId}";

            var details = await _cache.GetAsync<OutfitDetails>(cacheKey);
            if (details != null)
            {
                return details;
            }

            var outfit = await GetOutfitDetailsAsync(outfitId);
            if (outfit == null)
            {
                return null;
            }

            var outfitMembers = await GetOutfitMembers(outfitId);

            details = new OutfitDetails
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
                Activity7Days = outfitMembers.Count(a => (DateTime.UtcNow - a.LastLoginDate.GetValueOrDefault()) <= TimeSpan.FromDays(7)),
                Activity30Days = outfitMembers.Count(a => (DateTime.UtcNow - a.LastLoginDate.GetValueOrDefault()) <= TimeSpan.FromDays(30)),
                Activity90Days = outfitMembers.Count(a => (DateTime.UtcNow - a.LastLoginDate.GetValueOrDefault()) <= TimeSpan.FromDays(90))
            };

            await _cache.SetAsync(cacheKey, details, _cacheOutfitDetailsExpiration);

            return details;
        }

        public async Task<Outfit> GetOutfitDetailsAsync(string outfitId)
        {
            var outfit = await _outfitRepository.GetOutfitDetailsAsync(outfitId);

            if (outfit != null)
            {
                return outfit;
            }

            await GetOutfitAsync(outfitId);

            return await _outfitRepository.GetOutfitDetailsAsync(outfitId);
        }

        public async Task<OutfitDetails> GetOutfitByAlias(string outfitAlias)
        {
            var outfitId = await GetOutfitIdByAlias(outfitAlias);
            if (outfitId == null)
            {
                return null;
            }

            return await GetOutfitDetails(outfitId);
        }

        private async Task<string> GetOutfitIdByAlias(string outfitAlias)
        {
            var cacheKey = $"{_cacheKey}_name_{outfitAlias}";

            var outfitId = await _cache.GetAsync<string>(cacheKey);
            if (outfitId != null)
            {
                return outfitId;
            }

            outfitId = await _outfitRepository.GetOutfitIdByAlias(outfitAlias);
            if (outfitId != null)
            {
                await _cache.SetAsync(cacheKey, outfitId, _cacheOutfitNameExpiration);
            }

            return outfitId;
        }

        public async Task<IEnumerable<OutfitMemberDetails>> GetOutfitMembers(string outfitId)
        {
            var cacheKey = $"{_cacheKey}_member_details_{outfitId}";

            var memberDetails = await _cache.GetAsync<IEnumerable<OutfitMemberDetails>> (cacheKey);
            if (memberDetails != null)
            {
                return memberDetails;
            }

            var members = await _outfitRepository.GetOutfitMembersAsync(outfitId);

            memberDetails = members.Where(a => a.Character.Time != null).Select(a => new OutfitMemberDetails
            {
                CharacterId = a.CharacterId,
                MemberSinceDate = a.MemberSinceDate.Value,
                Rank = a.Rank,
                RankOrdinal = a.RankOrdinal.Value,
                Name = a.Character?.Name,
                BattleRank = a.Character?.BattleRank,
                LastLoginDate = a.Character?.Time?.LastLoginDate,
                LifetimeStats = new OutfitMemberDetailsStats
                {
                    FacilityCaptureCount = a.Character.LifetimeStats.FacilityCaptureCount,
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

            await _cache.SetAsync(cacheKey, memberDetails, _cacheOutfitMemberDetailsExpiration);

            return memberDetails;
        }

        public Task<IEnumerable<Outfit>> LookupOutfitsByName(string name, int limit = 12)
        {
            return _outfitRepository.GetOutfitsByNameAsync(name, limit);
        }

        public Task<Outfit> LookupOutfitByAlias(string alias)
        {
            return _outfitRepository.GetOutfitByAliasAsync(alias);
        }

        public async Task<OutfitMember> UpdateCharacterOutfitMembership(Character character)
        {
            OutfitMember outfitMember;

            using (await _outfitMembershipLock.WaitAsync(character.Id))
            {
                var cacheKey = $"{_cacheKey}_member_{character.Id}";

                outfitMember = await _cache.GetAsync<OutfitMember>(cacheKey);
                if (outfitMember != null)
                {
                    return outfitMember.OutfitId != null ? outfitMember : null;
                }

                CensusOutfitMemberModel membership;

                try
                {
                    membership = await _censusCharacter.GetCharacterOutfitMembership(character.Id);
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

                var outfit = await GetLatestOutfit(membership.OutfitId, character);
                if (outfit == null)
                {
                    _logger.LogError(84624, $"Unable to resolve outfit {membership.OutfitId} for character {character.Id}");
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

        private async Task<Outfit> GetLatestOutfit(string outfitId, Character character)
        {
            Outfit outfit;

            using (await _outfitLock.WaitAsync(outfitId))
            {
                outfit = await _cache.GetAsync<Outfit>(GetCacheKey(outfitId));
                if (outfit != null)
                {
                    return outfit;
                }

                outfit = await GetCensusOutfit(outfitId);
                if (outfit == null)
                {
                    return null;
                }

                outfit = await ResolveOutfitDetailsAsync(outfit, character);

                await _outfitRepository.UpsertAsync(outfit);
                await _cache.SetAsync(GetCacheKey(outfitId), outfit, _cacheOutfitExpiration);
            }

            return outfit;
        }

        private async Task<Outfit> GetOutfitAsync(string outfitId, Character member = null)
        {
            Outfit outfit;

            using (await _outfitLock.WaitAsync(outfitId))
            {
                outfit = await GetKnownOutfitAsync(outfitId);
                if (outfit == null)
                {
                    return null;
                }

                if (outfit.WorldId == null || outfit.FactionId == null)
                {
                    outfit = await ResolveOutfitDetailsAsync(outfit, member);
                    await _outfitRepository.UpsertAsync(outfit);
                }

                await _cache.SetAsync(GetCacheKey(outfitId), outfit, _cacheOutfitExpiration);
            }

            return outfit;
        }

        private async Task<Outfit> GetKnownOutfitAsync(string outfitId)
        {
            
            var outfit = await _cache.GetAsync<Outfit>(GetCacheKey(outfitId));
            if (outfit != null)
            {
                return outfit;
            }

            outfit = await _outfitRepository.GetOutfitAsync(outfitId);
            if (outfit == null)
            {
                try
                {
                    outfit = await GetCensusOutfit(outfitId);
                }
                catch (CensusConnectionException)
                {
                    return null;
                }
            }

            return outfit;
        }

        private async Task<Outfit> GetCensusOutfit(string outfitId)
        {
            var censusOutfit = await _censusOutfit.GetOutfit(outfitId);
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

        private async Task<Outfit> ResolveOutfitDetailsAsync(Outfit outfit, Character member = null)
        {
            if (member != null)
            {
                outfit.WorldId = member.WorldId;
                outfit.FactionId = member.FactionId;
            }
            else
            {
                var leader = await _censusCharacter.GetCharacter(member.Id);
                outfit.WorldId = leader.WorldId;
                outfit.FactionId = leader.FactionId;
            }

            return outfit;
        }

        private string GetCacheKey(string outfitId)
        {
            return $"{_cacheKey}_outfit_{outfitId}";
        }
    }
}
