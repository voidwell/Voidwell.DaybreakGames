using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.Census;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Test.Census;

/// <summary>
/// Stands in for the Census API. Responses are chosen per namespace and collection; anything unset returns an empty list.
/// </summary>
public sealed class FakeCensusHandler : HttpMessageHandler
{
    private readonly Dictionary<(string Namespace, string Collection), string> _responses = new();

    public List<(string Namespace, string Collection)> Requests { get; } = new();

    public HttpStatusCode? FailWith { get; set; }

    /// <summary>
    /// Namespaces that answer with a server error.
    /// </summary>
    public HashSet<string> FailingNamespaces { get; } = new();

    /// <summary>
    /// Sets the JSON objects returned for a collection on a namespace, e.g. <c>{"character_id":"1"}</c>.
    /// </summary>
    public FakeCensusHandler Respond(string censusNamespace, string collection, params string[] items)
    {
        _responses[(censusNamespace, collection)] = $"{{\"{collection}_list\":[{string.Join(",", items)}],\"returned\":{items.Length}}}";
        return this;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // /s:{serviceId}/get/{namespace}/{collection}/
        var segments = request.RequestUri!.AbsolutePath.Trim('/').Split('/');
        var key = (segments[2], segments[3]);

        lock (Requests)
        {
            Requests.Add(key);
        }

        if (FailWith != null || FailingNamespaces.Contains(key.Item1))
        {
            return Task.FromResult(new HttpResponseMessage(FailWith ?? HttpStatusCode.InternalServerError));
        }

        var json = _responses.TryGetValue(key, out var response) ? response : $"{{\"{key.Item2}_list\":[],\"returned\":0}}";

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
    }
}

public static class FakeCensus
{
    /// <summary>
    /// Registers the platform Census clients and the Census collections against <paramref name="handler"/>.
    /// </summary>
    public static IServiceCollection AddFakeCensus(this IServiceCollection services, FakeCensusHandler handler, params Platform[] enabledPlatforms)
    {
        services.AddLogging();
        services.AddCensusPlatforms(options =>
        {
            options.CensusServiceId = "test";
            options.Platforms = enabledPlatforms.Length > 0 ? enabledPlatforms : Enum.GetValues<Platform>();
        });

        foreach (var platform in Enum.GetValues<Platform>())
        {
            services.AddHttpClient(Microsoft.Extensions.DependencyInjection.CensusServiceExtensions.GetCensusHttpClientName(platform.ToString()))
                .ConfigurePrimaryHttpMessageHandler(() => handler);
        }

        services.AddCensusCollections();

        return services;
    }
}
