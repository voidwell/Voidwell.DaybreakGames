using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface ICharacterUpdaterRepository
    {
        Task AddAsync(CharacterUpdateQueue entity);
        Task RemoveAsync(CharacterUpdateQueue entity);
        Task<IEnumerable<CharacterUpdateQueue>> GetAllAsync();
    }
}