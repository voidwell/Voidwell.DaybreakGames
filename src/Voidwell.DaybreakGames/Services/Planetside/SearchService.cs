using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public class SearchService : ISearchService
    {
        private readonly ICharacterService _characterService;
        private readonly IOutfitService _outfitService;
        private readonly IItemService _itemService;
        private readonly ICache _cache;
        private readonly ILogger _logger;

        private readonly string _cacheKey = "ps2.search";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        public SearchService(ICharacterService characterService, IOutfitService outfitService, IItemService itemService, ICache cache, ILogger<SearchService> logger)
        {
            _characterService = characterService;
            _outfitService = outfitService;
            _itemService = itemService;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<SearchResult>> SearchPlanetside(string query)
        {
            var cacheResult = await _cache.GetAsync<List<SearchResult>>($"{_cacheKey}_{query}");
            if (cacheResult != null)
            {
                return cacheResult;
            }

            var lookupTasks = new List<Task>();

            Task<IEnumerable<CharacterSearchResult>> characterLookup = null;

            if (query.Length >= 3)
            {
                characterLookup = _characterService.LookupCharactersByName(query, 10);
                lookupTasks.Add(characterLookup);
            }

            var outfitNameLookup = _outfitService.LookupOutfitsByName(query, 5);
            var outfitAliasLookup = _outfitService.LookupOutfitByAlias(query);

            var weaponLookup = _itemService.LookupItemsByName(query, 5);

            lookupTasks.Add(outfitNameLookup);
            lookupTasks.Add(outfitAliasLookup);
            lookupTasks.Add(weaponLookup);

            await Task.WhenAll(lookupTasks);

            var characters = characterLookup?.Result ?? Enumerable.Empty<CharacterSearchResult>();
            var outfits = outfitNameLookup.Result.ToList();
            var weapons = weaponLookup.Result;

            var outfitAliasResult = outfitAliasLookup.Result; 
            if (outfitAliasResult != null && !outfits.Any(o => o.Id == outfitAliasResult.Id))
            {
                outfits.Add(outfitAliasResult);
            }

            var searchResults = new List<SearchResult>();

            foreach (var character in characters)
            {
                searchResults.Add(new SearchResult
                {
                    Type = "character",
                    Name = character.Name,
                    Id = character.Id,
                    FactionId = character.FactionId,
                    WorldId = character.WorldId,
                    BattleRank = character.BattleRank
                });
            }

            foreach (var outfit in outfits)
            {
                searchResults.Add(new SearchResult
                {
                    Type = "outfit",
                    Name = outfit.Name,
                    Id = outfit.Id,
                    FactionId = outfit.FactionId,
                    WorldId = outfit.WorldId,
                    Alias = outfit.Alias,
                    MemberCount = outfit.MemberCount
                });
            }

            foreach (var weapon in weapons)
            {
                searchResults.Add(new SearchResult
                {
                    Type = "item",
                    Name = weapon.Name,
                    Id = weapon.Id.ToString(),
                    FactionId = weapon.FactionId,
                    CategoryId = weapon.ItemCategoryId.ToString()
                });
            }

            var orderedResults = new List<SearchResult>();

            //Exact match outfit alias
            foreach (var result in searchResults)
            {
                if (result.Alias == query)
                {
                    orderedResults.Add(result);
                }
            }

            //Exact match
            foreach (var result in searchResults.Except(orderedResults))
            {
                if (result.Name == query)
                {
                    orderedResults.Add(result);
                }
            }

            //Starts with match
            foreach (var result in searchResults.Except(orderedResults))
            {
                if (result.Name.StartsWith(query))
                {
                    orderedResults.Add(result);
                }
            }

            //Case insensitive exact match
            foreach (var result in searchResults.Except(orderedResults))
            {
                if (result.Name.ToLower() == query.ToLower())
                {
                    orderedResults.Add(result);
                }
            }

            //Case insensitive starts with
            foreach (var result in searchResults.Except(orderedResults))
            {
                if (result.Name.ToLower().StartsWith(query.ToLower()))
                {
                    orderedResults.Add(result);
                }
            }

            //Case insensitive starts with
            foreach (var result in searchResults.Except(orderedResults))
            {
                if (result.Name.ToLower().StartsWith(query.ToLower()))
                {
                    orderedResults.Add(result);
                }
            }

            await _cache.SetAsync($"{_cacheKey}_{query}", orderedResults, _cacheExpiration);

            return orderedResults;
        }
    }
}
