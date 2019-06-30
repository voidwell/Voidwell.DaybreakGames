using Microsoft.Extensions.Logging;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Voidwell.Cache;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services
{
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
            return GetCachedFeed(_newsFeed, _newsCacheKey, _newsCacheExpiration);
        }

        public Task<IEnumerable<FeedItem>> GetUpdateFeed()
        {
            return GetCachedFeed(_updateFeed, _updatesCacheKey, _updatesCacheExpiration);
        }

        private async Task<IEnumerable<FeedItem>> GetCachedFeed(string feedUri, string cacheKey, TimeSpan cacheExpiration)
        {
            var feed = await _cache.GetAsync<IEnumerable<FeedItem>>(cacheKey);
            if (feed != null)
            {
                return feed;
            }

            _logger.LogInformation($"Fetching feed: {feedUri}");

            feed = await GetFeed(feedUri);
            if (feed != null)
            {
                await _cache.SetAsync(cacheKey, feed, cacheExpiration);
            }

            return feed;
        }

        private static async Task<IEnumerable<FeedItem>> GetFeed(string feedAddress)
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
}
