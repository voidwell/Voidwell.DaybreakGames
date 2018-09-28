using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.WebsocketServer
{
    public class WebSocketConnectionManager
    {
        private ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>> _sockets = new ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocket>>();

        public ConcurrentDictionary<string, WebSocket> GetSocketsByPath(string subscriptionPath)
        {
            var sockets = _sockets.GetValueOrDefault(subscriptionPath);
            if (sockets != null)
            {
                return sockets;
            }

            return new ConcurrentDictionary<string, WebSocket>();
        }

        public string GetId(WebSocket socket)
        {
            return _sockets.SelectMany(a => a.Value).FirstOrDefault(p => p.Value == socket).Key;
        }

        public string GetSubscription(string id)
        {
            foreach(var subscriptions in _sockets)
            {
                if (subscriptions.Value.ContainsKey(id))
                {
                    return subscriptions.Key;
                }
            }

            return null;
        }

        public void AddSocket(WebSocket socket, PathString path)
        {
            if (!_sockets.ContainsKey(path))
            {
                _sockets.TryAdd(path, new ConcurrentDictionary<string, WebSocket>());
            }

            _sockets[path].TryAdd(CreateConnectionId(), socket);
        }

        public async Task RemoveSocket(string id)
        {
            var subscription = GetSubscription(id);
            if (subscription == null)
            {
                return;
            }

            _sockets[subscription].TryRemove(id, out WebSocket socket);

            await socket.CloseAsync(closeStatus: WebSocketCloseStatus.NormalClosure,
                                    statusDescription: "Closed by the WebSocketManager",
                                    cancellationToken: CancellationToken.None);

            if (_sockets[subscription].IsEmpty)
            {
                _sockets.TryRemove(subscription, out ConcurrentDictionary<string, WebSocket> dic);
            }
        }

        private string CreateConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
