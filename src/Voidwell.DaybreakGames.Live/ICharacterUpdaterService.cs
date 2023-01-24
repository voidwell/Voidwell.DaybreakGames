using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Live
{
    public interface ICharacterUpdaterService
    {
        Task AddToQueue(string characterId);
    }
}
