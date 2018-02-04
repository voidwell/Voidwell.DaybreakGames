using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.Data.Repositories
{
    public interface ICharacterUpdaterRepository
    {
        Task AddAsync(CharacterUpdateQueue entity);
    }
}