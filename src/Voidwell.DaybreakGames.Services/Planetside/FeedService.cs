using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Voidwell.Common.Cache;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;
using Voidwell.DaybreakGames.Utils;

namespace Voidwell.DaybreakGames.Services.Planetside;

public class FeedService : IFeedService
{
    private readonly HttpClient _httpClient;
    private readonly ICache _cache;
    private readonly ILogger<FeedService> _logger;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromHours(1);
    private const string _newsCacheKey = "ps2.news";
    private const string _updatesCacheKey = "ps2.updates";
    private const string _patchNotesTag = "patchnotes";

    private const string _steamFeed = "https://api.steampowered.com/ISteamNews/GetNewsForApp/v0002/?appid=218230&count=50&format=json";
    private const string _steamNewsViewUrl = "https://store.steampowered.com/news/app/218230/view/";

    public FeedService(HttpClient httpClient, ICache cache, ILogger<FeedService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public Task<IEnumerable<FeedItem>> GetNewsFeed()
    {
        return GetCachedFeedAsync(_newsCacheKey, null)!;
    }

    public Task<IEnumerable<FeedItem>> GetUpdateFeed()
    {
        return GetCachedFeedAsync(_updatesCacheKey, _patchNotesTag)!;
    }

    private async Task<IEnumerable<FeedItem>?> GetCachedFeedAsync(string cacheKey, string? requiredTag)
    {
        return await _cache.GetOrSetIfNotNullAsync<IEnumerable<FeedItem>>(cacheKey, async ct =>
        {
            _logger.LogInformation("Fetching feed: {FeedUri}", _steamFeed);

            try
            {
                return await GetFeedAsync(requiredTag, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch feed: {FeedUri}", _steamFeed);
                throw;
            }
        }, _cacheExpiration);
    }

    private async Task<IEnumerable<FeedItem>> GetFeedAsync(string? requiredTag, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetFromJsonAsync<SteamNewsResponse>(_steamFeed, cancellationToken);
        var newsItems = response?.AppNews?.NewsItems ?? [];

        return newsItems
            .Where(i => requiredTag == null || (i.Tags?.Contains(requiredTag, StringComparer.OrdinalIgnoreCase) ?? false))
            .OrderByDescending(i => i.Date)
            .Select(i => new FeedItem
            {
                Title = i.Title,
                Content = BBCodeConverter.ToHtml(i.Contents),
                Date = DateTimeOffset.FromUnixTimeSeconds(i.Date),
                Link = _steamNewsViewUrl + i.Gid,
                Author = i.Author
            })
            .ToList();
    }

    private class SteamNewsResponse
    {
        [JsonPropertyName("appnews")]
        public SteamAppNews? AppNews { get; set; }
    }

    private class SteamAppNews
    {
        [JsonPropertyName("newsitems")]
        public List<SteamNewsItem>? NewsItems { get; set; }
    }

    private class SteamNewsItem
    {
        [JsonPropertyName("gid")]
        public string? Gid { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("contents")]
        public string? Contents { get; set; }

        [JsonPropertyName("date")]
        public long Date { get; set; }

        [JsonPropertyName("tags")]
        public List<string>? Tags { get; set; }
    }
}
