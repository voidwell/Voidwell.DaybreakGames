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

        private const string _cacheKeyPrefix = "ps2.search";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public SearchService(ICharacterService characterService, IOutfitService outfitService, IItemService itemService, ICache cache)
        {
            _characterService = characterService;
            _outfitService = outfitService;
            _itemService = itemService;
            _cache = cache;
        }

        public async Task<IEnumerable<SearchResult>> SearchPlanetside(string category, string query)
        {
            var cacheKey = $"{_cacheKeyPrefix}_{category}_{query}";

            var cacheResult = await _cache.GetAsync<List<SearchResult>>(cacheKey);
            if (cacheResult != null)
            {
                return cacheResult;
            }

            var searchResults = new List<SearchResult>();

            switch (category)
            {
                case "character":
                    var characters = await _characterService.LookupCharactersByName(query, 10);

                    if (characters != null)
                    {
                        foreach (var character in characters)
                        {
                            searchResults.Add(new SearchResult
                            {
                                Type = category,
                                Name = character.Name,
                                Id = character.Id,
                                FactionId = character.FactionId,
                                WorldId = character.WorldId,
                                BattleRank = character.BattleRank
                            });
                        }
                    }

                    break;
                case "outfit":
                    var outfitNameLookup = _outfitService.LookupOutfitsByName(query, 10);
                    var outfitAliasLookup = _outfitService.LookupOutfitByAlias(query);

                    await Task.WhenAll(outfitNameLookup, outfitAliasLookup);

                    var outfits = outfitNameLookup.Result.ToList();

                    var outfitAliasResult = outfitAliasLookup.Result;
                    if (outfitAliasResult != null && outfits.All(o => o.Id != outfitAliasResult.Id))
                    {
                        outfits.Add(outfitAliasResult);
                    }

                    foreach (var outfit in outfits)
                    {
                        searchResults.Add(new SearchResult
                        {
                            Type = category,
                            Name = outfit.Name,
                            Id = outfit.Id,
                            FactionId = outfit.FactionId,
                            WorldId = outfit.WorldId,
                            Alias = outfit.Alias,
                            MemberCount = outfit.MemberCount
                        });
                    }

                    break;
                case "item":
                    var weapons = await _itemService.LookupWeaponsByName(query, 10);

                    foreach (var weapon in weapons)
                    {
                        searchResults.Add(new SearchResult
                        {
                            Type = category,
                            Name = weapon.Name,
                            Id = weapon.Id.ToString(),
                            FactionId = weapon.FactionId,
                            CategoryId = weapon.ItemCategoryId.ToString()
                        });
                    }

                    break;
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
                if (string.Equals(result.Name, query, StringComparison.CurrentCultureIgnoreCase))
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

            //Everything else
            orderedResults.AddRange(searchResults.Except(orderedResults));

            await _cache.SetAsync(cacheKey, orderedResults, _cacheExpiration);

            return orderedResults;
        }
    }
}
