using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Voidwell.DaybreakGames.Api.Authentication;

public class AuthenticatedHttpClientOptions
{
    public string? TokenServiceAddress { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public List<string> Scopes { get; set; } = new List<string>();
}

public interface IHttpTokenManager
{
    Task<string?> GetTokenAsync(string clientName);
}

public class HttpTokenManager : IHttpTokenManager, IDisposable
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<AuthenticatedHttpClientOptions> _optionsMonitor;
    private readonly ILogger<HttpTokenManager> _logger;

    private readonly Dictionary<string, TokenState> _tokens = new Dictionary<string, TokenState>();
    private readonly SemaphoreSlim _tokenSemaphore = new SemaphoreSlim(1);

    public HttpTokenManager(IHttpClientFactory httpClientFactory, IOptionsMonitor<AuthenticatedHttpClientOptions> optionsMonitor, ILogger<HttpTokenManager> logger)
    {
        _httpClientFactory = httpClientFactory;
        _optionsMonitor = optionsMonitor;
        _logger = logger;
    }

    public async Task<string?> GetTokenAsync(string clientName)
    {
        if (!IsTokenValid(clientName))
        {
            await UpdateTokenAsync(clientName);
        }

        return _tokens.TryGetValue(clientName, out var token) ? token.AccessToken : null;
    }

    private async Task UpdateTokenAsync(string clientName)
    {
        await _tokenSemaphore.WaitAsync();

        try
        {
            if (IsTokenValid(clientName))
            {
                return;
            }

            _tokens.Remove(clientName);

            var response = await RequestNewToken(clientName);
            response.EnsureSuccessStatusCode();

            var tokenNode = JsonNode.Parse(await response.Content.ReadAsStringAsync());
            var accessToken = tokenNode!["access_token"]!.GetValue<string>();
            var expiresIn = tokenNode["expires_in"]!.GetValue<int>();

            _tokens[clientName] = new TokenState(accessToken, expiresIn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve new token for '{ClientName}'", clientName);
        }
        finally
        {
            _tokenSemaphore.Release();
        }
    }

    private async Task<HttpResponseMessage> RequestNewToken(string clientName)
    {
        var clientOptions = _optionsMonitor.Get(clientName);

        var payload = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientOptions.ClientId! },
            { "client_secret", clientOptions.ClientSecret! },
            { "scope", string.Join(" ", clientOptions.Scopes) }
        };

        var httpClient = _httpClientFactory.CreateClient(nameof(HttpTokenManager));
        using var content = new FormUrlEncodedContent(payload);
        return await httpClient.PostAsync(clientOptions.TokenServiceAddress, content);
    }

    private bool IsTokenValid(string clientName)
    {
        return _tokens.TryGetValue(clientName, out var token) && !token.IsExpired();
    }

    public void Dispose()
    {
        _tokenSemaphore.Dispose();
    }

    private class TokenState
    {
        public TokenState(string accessToken, int expiresIn)
        {
            AccessToken = accessToken;
            Expiration = DateTimeOffset.UtcNow.AddSeconds(expiresIn);
        }

        public string AccessToken { get; }
        public DateTimeOffset Expiration { get; }

        public bool IsExpired()
        {
            return string.IsNullOrWhiteSpace(AccessToken) || DateTimeOffset.UtcNow > Expiration.AddMinutes(-5);
        }
    }
}

public class AuthenticatedHttpMessageHandler<TClient> : DelegatingHandler where TClient : class
{
    private readonly IHttpTokenManager _httpTokenManager;
    private readonly string _sourceClient = typeof(TClient).FullName!;

    public AuthenticatedHttpMessageHandler(IHttpTokenManager httpTokenManager)
    {
        _httpTokenManager = httpTokenManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _httpTokenManager.GetTokenAsync(_sourceClient);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}

public static class AuthenticatedHttpClientExtensions
{
    public static void AddAuthenticatedHttpClient<TClient, TImplementation>(this IServiceCollection services, Action<AuthenticatedHttpClientOptions> options)
        where TClient : class
        where TImplementation : class, TClient
    {
        services.AddHttpClient(nameof(HttpTokenManager));

        services.AddOptions();
        services.Configure(typeof(TClient).FullName, options);

        services.TryAddSingleton<IHttpTokenManager, HttpTokenManager>();
        services.TryAddTransient<AuthenticatedHttpMessageHandler<TClient>>();

        services.AddHttpClient<TClient, TImplementation>()
            .AddHttpMessageHandler<AuthenticatedHttpMessageHandler<TClient>>();
    }
}
