using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface ICharacterUpdaterService
    {
        Task AddToQueue(string characterId);
        Task<int> GetQueueLengthAsync();
        Task<IEnumerable<CharacterUpdateQueue>> GetAllAsync(TimeSpan delay);
        Task RemoveAsync(CharacterUpdateQueue queueItem);
    }
}