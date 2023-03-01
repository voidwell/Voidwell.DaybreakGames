﻿using DaybreakGames.Census;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Census.Collection.Abstract;
using Voidwell.DaybreakGames.Census.Models;

namespace Voidwell.DaybreakGames.Census.Collection
{
    public class CharactersDirectiveCollection : CensusCollection
    {
        public override string CollectionName => "characters_directive";

        public CharactersDirectiveCollection(ICensusClient censusClient) : base(censusClient)
        {
        }

        public async Task<IEnumerable<CensusCharacterDirectiveModel>> GetCharacterDirectivesAsync(string characterId)
        {
            return await QueryAsync(query =>
            {
                query.Where("character_id").Equals(characterId);

                return query.GetBatchAsync<CensusCharacterDirectiveModel>();
            });
        }
    }
}
