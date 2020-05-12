using System.Threading.Tasks;
using Voidwell.DaybreakGames.Messaging.Models;

namespace Voidwell.DaybreakGames.Messaging
{
    public interface IMessageService
    {
        Task PublishCharacterEvent<T>(string characterId, T message) where T : PlanetsideCharacterMessage;
        Task PublishAlertEvent<T>(int worldId, int instanceId, T message) where T : PlanetsideAlertMessage;
    }
}