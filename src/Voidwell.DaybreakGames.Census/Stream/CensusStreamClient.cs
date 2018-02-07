using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Census.Stream
{
    public class CensusStreamClient : IDisposable
    {
        private const int _chunkSize = 1024;
        private UTF8Encoding encoder = new UTF8Encoding();
        private JsonSerializerSettings sendMessageSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        private ClientWebSocket _client;

        private Timer _timer;
        private readonly TimeSpan _stateCheckInterval = TimeSpan.FromSeconds(5);

        private string _apiKey { get; set; }
        private string _apiNamespace { get; set; }
        private CensusStreamSubscription _subscription { get; set; }

        public CensusStreamClient(CensusStreamSubscription subscription, string apiKey = Constants.DefaultApiKey, string apiNamespace = Constants.DefaultApiNamespace)
        {
            if(subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            _apiKey = apiKey;
            _apiNamespace = apiNamespace;

            _client = new ClientWebSocket();
            _subscription = subscription;
        }

        public CensusStreamState GetState()
        {
            if (Enum.TryParse(_client.State.ToString(), out CensusStreamState state))
            {
                return state;
            }

            return CensusStreamState.None;
        }

        public async Task ConnectAsync(CancellationToken ct)
        {
            _timer?.Dispose();

            await _client.ConnectAsync(GetEndpoint(), ct);

            if (_client.State == WebSocketState.Open)
            {
                await SendAsync(_subscription, ct);
            }

            SetupReconnect();
        }

        public async Task<JToken> ReceiveAsync(CancellationToken cancellationToken)
        {
            var message = await ReceiveMessageAsync();
            if (message == null)
            {
                return null;
            }

            try
            {
                return JToken.Parse(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex);
                Console.WriteLine(message);
            }

            return null;
        }

        public async Task CloseAsync(CancellationToken ct)
        {
            _timer?.Dispose();

            if (_client.State == WebSocketState.Open || _client.State == WebSocketState.Connecting)
            {
                await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close requested by client", ct);
            }
        }

        private Task SendAsync(object message, CancellationToken cancellationToken)
        {
            if (_client.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("Connection is not ready to send data. Wait for connect to be in open state.");
            }

            var sMessage = JsonConvert.SerializeObject(message, sendMessageSettings);
            byte[] buffer = encoder.GetBytes(sMessage);
            var seg = new ArraySegment<byte>(buffer);

            return _client.SendAsync(seg, WebSocketMessageType.Text, true, cancellationToken);
        }

        private async Task<string> ReceiveMessageAsync()
        {
            var buffer = new byte[_chunkSize];
            var seg = new ArraySegment<byte>(buffer);
            WebSocketReceiveResult result = null;

            using (var ms = new MemoryStream())
            {
                do
                {
                    result = await _client.ReceiveAsync(seg, CancellationToken.None);
                    ms.Write(seg.Array, seg.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }

            return null;
        }

        private Uri GetEndpoint()
        {
            return new Uri($"{Constants.WebsocketEndpoint}?environment={_apiNamespace}&service-id=s:{_apiKey}");
        }

        private void SetupReconnect()
        {
            _timer?.Dispose();

            _timer = new Timer(ReconnectClientAsync, null, (int)_stateCheckInterval.TotalMilliseconds, (int)_stateCheckInterval.TotalMilliseconds);
        }

        private async void ReconnectClientAsync(Object stateInfo)
        {
            var state = _client.State;

            if (state == WebSocketState.Closed || state == WebSocketState.Aborted || state == WebSocketState.None)
            {
                Console.WriteLine($"Census stream client is closed. Attempting reconnect: {_client.CloseStatusDescription}");

                await ConnectAsync(CancellationToken.None);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _client.Dispose();
        }
    }
}
