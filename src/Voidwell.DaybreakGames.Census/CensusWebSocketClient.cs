using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.Census
{
    public class CensusWebSocketClient : IDisposable
    {
        private ClientWebSocket _client;

        private string CensusNamespace { get; set; } = "ps2";
        private string CensusKey { get; set; } = "example";

        private const int receiveChunkSize = 1024;
        private UTF8Encoding encoder;
        private JsonSerializerSettings sendMessageSettings;

        public CensusWebSocketClient(string censusKey)
        {
            CensusKey = censusKey;

            _client = new ClientWebSocket();

            encoder = new UTF8Encoding();
            sendMessageSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public bool IsConnected()
        {
            return _client.State == WebSocketState.Open;
        }

        public bool IsConnecting()
        {
            return _client.State == WebSocketState.Connecting;
        }

        public bool IsClosed()
        {
            return _client.State == WebSocketState.Closed;
        }

        public bool IsClosing()
        {
            return _client.State == WebSocketState.CloseSent || _client.State == WebSocketState.CloseReceived;
        }

        public WebSocketState GetState()
        {
            return _client.State;
        }

        public WebSocketCloseStatus? CloseStatus
        {
            get
            {
                return _client.CloseStatus;
            }
        }

        public Task ConnectAsync()
        {
            return ConnectAsync(CancellationToken.None);
        }

        public Task ConnectAsync(CancellationToken cancellationToken)
        {
            var censusEndpoint = new Uri($"{Constants.CensusWebsocketEndpoint}?environment={CensusNamespace}&service-id=s:{CensusKey}");
            return _client.ConnectAsync(censusEndpoint, cancellationToken);
        }

        public Task Subscribe(params string[] eventNames)
        {
            return Subscribe(null, null, eventNames);
        }

        public Task Subscribe(IEnumerable<string> worldIds, params string[] eventNames)
        {
            return Subscribe(null, worldIds, eventNames);
        }

        public Task Subscribe(IEnumerable<string> characterIds, IEnumerable<string> worldIds, IEnumerable<string> eventNames)
        {
            var subscription = new CensusWebsocketSubscription
            {
                Service = "event",
                Action = "subscribe",
                Characters = characterIds,
                Worlds = worldIds,
                EventNames = eventNames
            };

            return Subscribe(subscription);
        }

        public Task Subscribe(CensusWebsocketSubscription subscription)
        {
            return SendAsync(subscription);
        }

        public Task SendAsync(object message)
        {
            return SendAsync(message, CancellationToken.None);
        }

        public Task SendAsync(object message, CancellationToken cancellationToken)
        {
            var sMessage = JsonConvert.SerializeObject(message, sendMessageSettings);
            byte[] buffer = encoder.GetBytes(sMessage);
            var seg = new ArraySegment<byte>(buffer);
            return _client.SendAsync(seg, WebSocketMessageType.Text, true, cancellationToken);
        }

        public Task<CensusWebSocketReceiveResult> ReceiveAsync()
        {
            return ReceiveAsync(CancellationToken.None);
        }

        public Task<CensusWebSocketReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[receiveChunkSize];
            var seg = new ArraySegment<byte>(buffer);
            return ReceiveAsync(seg, cancellationToken);
        }

        public Task<CensusWebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer)
        {
            return ReceiveAsync(buffer, CancellationToken.None);
        }

        public async Task<CensusWebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            var result = await _client.ReceiveAsync(buffer, cancellationToken);

            var bufferString = encoder.GetString(buffer.Array);
            JToken jBuffer = null;

            try
            {
                jBuffer = JToken.Parse(bufferString);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
                Console.WriteLine(bufferString);
            }

            return new CensusWebSocketReceiveResult
            {
                Result = result,
                Content = jBuffer
            };
        }

        public Task CloseAsync()
        {
            return CloseAsync(CancellationToken.None);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return CloseAsync(WebSocketCloseStatus.NormalClosure, "Client requested close", cancellationToken);
        }

        public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription)
        {
            return CloseAsync(closeStatus, statusDescription, CancellationToken.None);
        }

        public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
        {
            return _client.CloseAsync(closeStatus, statusDescription, cancellationToken);
        }

        public void Abort()
        {
            _client.Abort();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }

    public class CensusWebSocketReceiveResult
    {
        public JToken Content { get; set; }
        public WebSocketReceiveResult Result { get; set; }
    }

    public class CensusWebsocketSubscription
    {
        public string Service { get; set; }
        public string Action { get; set; }
        public IEnumerable<string> Characters { get; set; }
        public IEnumerable<string> Worlds { get; set; }
        public IEnumerable<string> EventNames { get; set; }
    }
}
