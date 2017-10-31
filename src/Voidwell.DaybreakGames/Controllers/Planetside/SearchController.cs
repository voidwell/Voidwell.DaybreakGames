using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;
using Voidwell.DaybreakGames.Services.Planetside;

namespace Voidwell.DaybreakGames.Controllers.Planetside
{
    [Route("ps2/alert")]
    public class SearchController : Controller
    {
        private readonly ICharacterService _characterService;
        private readonly IOutfitService _outfitService;
        private readonly IItemService _itemService;

        public SearchController(ICharacterService characterService, IOutfitService outfitService, IItemService itemService)
        {
            _characterService = characterService;
            _outfitService = outfitService;
            _itemService = itemService;
        }

        [HttpGet("{query}")]
        public async Task<ActionResult> Search(string query)
        {
            var characterLookup = _characterService.LookupCharactersByName(query, 10);
            var outfitLookup = _outfitService.LookupOutfitsByName(query, 10);
            var weaponLookup = _itemService.LookupItemsByName(query, 10);

            await Task.WhenAll(characterLookup, outfitLookup, weaponLookup);

            var characters = characterLookup.Result;
            var outfits = outfitLookup.Result;
            var weapons = weaponLookup.Result;

            var searchResults = new List<SearchResult>();

            foreach(var character in characters)
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
                    Id = weapon.Id,
                    FactionId = weapon.FactionId,
                    CategoryId = weapon.ItemCategoryId
                });
            }

            var orderedResults = new List<SearchResult>();

            //Exact match
            foreach (var result in searchResults)
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

            return Ok(orderedResults);
        }
    }
}
