using System.Threading.Tasks;
using Voidwell.DaybreakGames.Messages.Models;

namespace Voidwell.DaybreakGames.Messages
{
    public interface IMessageService
    {
        Task PublishCharacterEvent<T>(string characterId, T message) where T : PlanetsideCharacterMessage;
        Task PublishAlertEvent<T>(int worldId, int instanceId, T message) where T : PlanetsideAlertMessage;
    }
}