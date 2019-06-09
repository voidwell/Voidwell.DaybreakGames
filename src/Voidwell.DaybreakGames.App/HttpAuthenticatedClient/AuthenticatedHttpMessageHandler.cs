using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.HttpAuthenticatedClient
{
    public class AuthenticatedHttpMessageHandler : HttpMessageHandler
    {
        private readonly AuthenticatedHttpClientOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;

        private readonly SemaphoreSlim _semaphoreSlim;
        private readonly HttpMessageInvoker _httpMessageInvoker;
        private CustomTokenClient _tokenClient;

        private DateTimeOffset? _resetTokenAfter;
        private string _token;

        public AuthenticatedHttpMessageHandler(AuthenticatedHttpClientOptions options,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthenticatedHttpMessageHandler> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            _tokenClient = new CustomTokenClient(_options.TokenServiceAddress,
                        _options.ClientId,
                        _options.TokenServiceMessageHandler ?? new HttpClientHandler(),
                        AuthenticationStyle.BasicAuthentication);

            _semaphoreSlim = new SemaphoreSlim(1, 1);
            _resetTokenAfter = null;

            var handler = options.InnerMessageHandler ?? new HttpClientHandler();
            _httpMessageInvoker = new HttpMessageInvoker(handler, true);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken callerCancellationToken)
        {
            CancellationTokenSource messageHandlerTimeoutCancellationTokenSource =
                new CancellationTokenSource(_options.MessageHandlerTimeout);
            CancellationTokenSource linkedCancelationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                callerCancellationToken, messageHandlerTimeoutCancellationTokenSource.Token);

            try
            {
                await GetTokenIfNeededAsync(linkedCancelationTokenSource.Token);

                CancellationTokenSource cancellationTokenWithHttpAbortedSource = CancellationTokenSource.CreateLinkedTokenSource(
                    linkedCancelationTokenSource.Token, _httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None);

                return await SetAuthAndSendAsync(request, cancellationTokenWithHttpAbortedSource.Token);
            }
            catch (TokenServiceResponseException ex)
            when (messageHandlerTimeoutCancellationTokenSource.Token.IsCancellationRequested && ex.TokenResponse.Exception is OperationCanceledException)
            {
                // When the cancelation happens within the token service, the exception is wrapped
                //Only catches when messageHandlerTimeoutCancellationTokenSource is canceled, not when caller cancels
                var msg =
                    $"{nameof(AuthenticatedHttpMessageHandler)} exceeded the timed out of {_options.MessageHandlerTimeout} ms when making a request to {request.Method.Method.ToUpper()} {request.RequestUri}";

                throw new TimeoutException(msg, ex);
            }
            catch (OperationCanceledException ex) when (messageHandlerTimeoutCancellationTokenSource.Token.IsCancellationRequested)
            {
                //Only catches when messageHandlerTimeoutCancellationTokenSource is canceled, not when caller cancels
                var msg =
                    $"{nameof(AuthenticatedHttpMessageHandler)} exceeded the timed out of {_options.MessageHandlerTimeout} ms when making a request to {request.Method.Method.ToUpper()} {request.RequestUri}";

                throw new TimeoutException(msg, ex);
            }
        }

        public Task<bool> GetTokenIfNeededAsync(CancellationToken token)
        {
            // Requests from outside (for warming) are not part of incoming requests and 
            // it is desired to reduce logging noise here
            return GetTokenIfNeededAsync(false, token);
        }

        private Task<bool> GetTokenIfNeededAsync(bool isIncomingRequest, CancellationToken token)
        {
            return IsNewTokenNeeded() ? GetTokenAsync(isIncomingRequest, token) : Task.FromResult(false);
        }

        private Task<HttpResponseMessage> SetAuthAndSendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            return _httpMessageInvoker.SendAsync(request, cancellationToken);
        }

        private async Task<bool> GetTokenAsync(bool isIncomingRequest, CancellationToken callerCancellationToken)
        {
            CancellationTokenSource tokenServiceTimeoutCancellationTokenSource = null;
            var timer = Stopwatch.StartNew();

            if (isIncomingRequest)
            {
                // Don't log this message when warming
                _logger.LogInformation(22, null, "Getting token for client {0}", _options.ClientId);
            }

            await _semaphoreSlim.WaitAsync(callerCancellationToken);

            try
            {
                if (IsNewTokenNeeded())
                {
                    // Starts timer on local cancellation source
                    tokenServiceTimeoutCancellationTokenSource = new CancellationTokenSource(_options.TokenServiceTimeout);

                    // Creates a cancelation source 
                    CancellationTokenSource linkedCancellationTokenSource = CancellationTokenSource
                        .CreateLinkedTokenSource(callerCancellationToken, tokenServiceTimeoutCancellationTokenSource.Token);

                    _resetTokenAfter = null;

                    // Build scope string
                    var scopes = string.Join(" ", _options.Scopes);
                    _tokenClient.ClientSecret = _options.ClientSecret;

                    var response = await _tokenClient.RequestClientCredentialsAsync(scopes,
                        cancellationToken: linkedCancellationTokenSource.Token);

                    ValidateTokenResponse(response);

                    _token = response.AccessToken;
                    _resetTokenAfter = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn);

                    _logger.LogInformation(23, null, "Token found for client {0}. Expires at {1} ({2} ms)",
                        _options.ClientId, _resetTokenAfter.Value, timer.ElapsedMilliseconds);
                }
                else if (isIncomingRequest)
                {
                    _logger.LogInformation(24, null, "Using token from different request for client {0}. Expires at {1} ({2} ms)",
                        _options.ClientId, _resetTokenAfter.Value, timer.ElapsedMilliseconds);
                }
                else
                {
                    // Did not get token
                    return false;
                }
            }
            catch (TokenServiceResponseException ex) when (tokenServiceTimeoutCancellationTokenSource.Token.IsCancellationRequested)
            {
                //Only catches when tokenServiceTimeoutCancellationTokenSource is canceled, not when caller cancels
                string msg = string.Format("{0} exceeded the time out of {1} ms when attempting to call token service.",
                    nameof(AuthenticatedHttpMessageHandler), _options.TokenServiceTimeout);

                _logger.LogError(25, ex, msg);

                throw new TimeoutException(msg, ex);
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            return true;
        }

        public bool IsNewTokenNeeded()
        {
            return !_resetTokenAfter.HasValue ||
                _resetTokenAfter.Value.AddMinutes(-5) < DateTimeOffset.UtcNow ||
                string.IsNullOrWhiteSpace(_token);
        }

        private void ValidateTokenResponse(TokenResponse response)
        {
            if (response.IsError)
            {
                var msg = $"The token service had an error.\r\nClient ID: {_options.ClientId}\r\nStatusCode: {response.HttpStatusCode}\r\nHttpErrorReason: {response.HttpErrorReason}";
                msg += $"Error {response.Error}\r\nError Description: {response.ErrorDescription}\r\nError Type: {response.ErrorType}";

                throw new TokenServiceResponseException(response, msg);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _semaphoreSlim.Dispose();
                _httpMessageInvoker.Dispose();
                _tokenClient.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
