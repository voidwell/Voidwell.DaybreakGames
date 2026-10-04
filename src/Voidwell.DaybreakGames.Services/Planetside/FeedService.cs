using System.Xml;
using Microsoft.Extensions.Logging;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using Voidwell.DaybreakGames.Cache;
using Voidwell.DaybreakGames.Domain.Models;
using Voidwell.DaybreakGames.Services.Planetside.Abstractions;

namespace Voidwell.DaybreakGames.Services.Planetside;

public class FeedService : IFeedService
{
    private readonly ICache _cache;
    private readonly ILogger<FeedService> _logger;
    private readonly TimeSpan _newsCacheExpiration = TimeSpan.FromHours(1);
    private readonly TimeSpan _updatesCacheExpiration = TimeSpan.FromHours(1);
    private const string _newsCacheKey = "ps2.news";
    private const string _updatesCacheKey = "ps2.updates";

    private const string _newsFeed = "https://forums.daybreakgames.com/ps2/index.php?forums/official-news-and-announcements.19/index.rss";
    private const string _updateFeed = "https://forums.daybreakgames.com/ps2/index.php?forums/game-update-notes.73/index.rss";

    public FeedService(ICache cache, ILogger<FeedService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<IEnumerable<FeedItem>> GetNewsFeed()
    {
        return GetCachedFeedAsync(_newsFeed, _newsCacheKey, _newsCacheExpiration)!;
    }

    public Task<IEnumerable<FeedItem>> GetUpdateFeed()
    {
        return GetCachedFeedAsync(_updateFeed, _updatesCacheKey, _updatesCacheExpiration)!;
    }

    private async Task<IEnumerable<FeedItem>?> GetCachedFeedAsync(string feedUri, string cacheKey, TimeSpan cacheExpiration)
    {
        return await _cache.GetOrSetIfNotNullAsync<IEnumerable<FeedItem>>(cacheKey, async ct =>
        {
            _logger.LogInformation("Fetching feed: {FeedUri}", feedUri);

            return await GetFeedAsync(feedUri);
        }, cacheExpiration);
    }

    private static async Task<IEnumerable<FeedItem>> GetFeedAsync(string feedAddress)
    {
        var feedResult = new List<FeedItem>();

        using (var xmlReader = XmlReader.Create(feedAddress, new XmlReaderSettings { Async = true }))
        {
            var feedReader = new RssFeedReader(xmlReader);

            while (await feedReader.Read())
            {
                if (feedReader.ElementType == SyndicationElementType.Item)
                {
                    var item = await feedReader.ReadItem();

                    var feedItem = new FeedItem
                    {
                        Title = item.Title,
                        Content = item.Description,
                        Date = item.Published,
                        Link = item.Links?.FirstOrDefault(l => l.RelationshipType == "guid")?.Uri.ToString(),
                        Author = item.Contributors?.FirstOrDefault(c => c.RelationshipType == "author")?.Email
                    };

                    feedResult.Add(feedItem);
                }
            }
        }

        return feedResult;
    }
}
