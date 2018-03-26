using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.Census.Stream
{
    public class WebSocketWrapper : IDisposable
    {
        private const int ReceiveChunkSize = 1024;
        private const int SendChunkSize = 1024;

        private readonly ClientWebSocket _ws;
        private readonly Uri _uri;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly CancellationToken _cancellationToken;

        private Action<string, WebSocketWrapper> _onMessage;
        private Action<string, WebSocketWrapper> _onDisconnected;

        protected WebSocketWrapper(string uri)
        {
            _ws = new ClientWebSocket();
            _ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
            _uri = new Uri(uri);
            _cancellationToken = _cancellationTokenSource.Token;
        }

        public WebSocketState State => _ws?.State ?? WebSocketState.Closed;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="uri">The URI of the WebSocket server.</param>
        /// <returns></returns>
        public static WebSocketWrapper Create(string uri)
        {
            return new WebSocketWrapper(uri);
        }

        /// <summary>
        /// Connects to the WebSocket server.
        /// </summary>
        /// <returns></returns>
        public async Task<WebSocketWrapper> Connect()
        {
            await ConnectAsync();
            return this;
        }

        /// <summary>
        /// Disconnects from the WebSocket server.
        /// </summary>
        /// <returns></returns>
        public async Task<WebSocketWrapper> Disconnect()
        {
            await DisconnectAsync();
            return this;
        }

        /// <summary>
        /// Set the Action to call when the connection has been terminated.
        /// </summary>
        /// <param name="onDisconnect">The Action to call</param>
        /// <returns></returns>
        public WebSocketWrapper OnDisconnect(Action<string, WebSocketWrapper> onDisconnect)
        {
            _onDisconnected = onDisconnect;
            return this;
        }

        /// <summary>
        /// Set the Action to call when a messages has been received.
        /// </summary>
        /// <param name="onMessage">The Action to call.</param>
        /// <returns></returns>
        public WebSocketWrapper OnMessage(Action<string, WebSocketWrapper> onMessage)
        {
            _onMessage = onMessage;
            return this;
        }

        /// <summary>
        /// Send a message to the WebSocket server.
        /// </summary>
        /// <param name="message">The message to send</param>
        public Task SendMessage(string message)
        {
            return SendMessageAsync(message);
        }

        private async Task SendMessageAsync(string message)
        {
            if (_ws.State != WebSocketState.Open)
            {
                throw new Exception("Connection is not open.");
            }

            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var messagesCount = (int)Math.Ceiling((double)messageBuffer.Length / SendChunkSize);

            for (var i = 0; i < messagesCount; i++)
            {
                var offset = (SendChunkSize * i);
                var count = SendChunkSize;
                var lastMessage = ((i + 1) == messagesCount);

                if ((count * (i + 1)) > messageBuffer.Length)
                {
                    count = messageBuffer.Length - offset;
                }

                await _ws.SendAsync(new ArraySegment<byte>(messageBuffer, offset, count), WebSocketMessageType.Text, lastMessage, _cancellationToken);
            }
        }

        private async Task ConnectAsync()
        {
            await _ws.ConnectAsync(_uri, _cancellationToken);
            StartListening();
        }

        private async Task DisconnectAsync()
        {
            try
            {
                await _ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                CallOnDisconnected("Application normal closure");
            }
            finally
            {
                _ws?.Dispose();
            }
        }

        private async Task StartListen()
        {
            var buffer = new byte[ReceiveChunkSize];

            try
            {
                while (_ws.State == WebSocketState.Open)
                {
                    var stringResult = new StringBuilder();


                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationToken);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await DisconnectAsync();
                        }
                        else
                        {
                            var str = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            stringResult.Append(str);
                        }

                    } while (!result.EndOfMessage);

                    CallOnMessage(stringResult);

                }
            }
            catch(Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException is ObjectDisposedException)
                    return;

                CallOnDisconnected(ex.Message);
            }
            finally
            {
                _ws?.Dispose();
            }
        }

        private void CallOnMessage(StringBuilder stringResult)
        {
            if (_onMessage != null)
                RunInTask(() => _onMessage(stringResult.ToString(), this));
        }

        private void CallOnDisconnected(string error = null)
        {
            if (_onDisconnected != null)
                RunInTask(() => _onDisconnected(error, this));
        }

        private void StartListening()
        {
            Task.Run(() => StartListen());
        }

        private static void RunInTask(Action action)
        {
            Task.Run(action);
        }

        public void Dispose()
        {
            _ws?.Dispose();
        }
    }
}
