﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories.Abstractions
{
    public interface ICharacterUpdaterRepository
    {
        Task AddAsync(CharacterUpdateQueue entity);
        Task RemoveAsync(CharacterUpdateQueue entity);
        Task<IEnumerable<CharacterUpdateQueue>> GetAllAsync(TimeSpan? delay = null);
        Task<int> GetQueueLengthAsync(TimeSpan? delay = null);
    }
}