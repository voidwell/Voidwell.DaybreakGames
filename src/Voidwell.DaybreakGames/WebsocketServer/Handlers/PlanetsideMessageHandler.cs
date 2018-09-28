using System.Threading.Tasks;
using Voidwell.DaybreakGames.WebsocketServer.Models;

namespace Voidwell.DaybreakGames.WebsocketServer.Handlers
{
    public class PlanetsideMessageHandler : WebSocketHandler
    {
        public PlanetsideMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public async Task SendOnlineCharacterMessage(string characterId, PlanetsideMessage message)
        {
            await SendMessageAsync($"/ws/ps2/online_character/{characterId}", message.ToString());
        }
    }
}
