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
        private readonly TimeSpan _newsCacheExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _updatesCacheExpiration = TimeSpan.FromMinutes(30);
        private readonly string _newsCacheKey = "ps2.news";
        private readonly string _updatesCacheKey = "ps2.updates";

        private const string _newsFeed = "https://forums.daybreakgames.com/ps2/index.php?forums/official-news-and-announcements.19/index.rss";
        private const string _updateFeed = "https://forums.daybreakgames.com/ps2/index.php?forums/game-update-notes.73/index.rss";

        public FeedService(ICache cache, ILogger<FeedService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<FeedItem>> GetNewsFeed()
        {
            var cacheNews = await _cache.GetAsync<List<FeedItem>>(_newsCacheKey);
            if (cacheNews != null)
            {
                return cacheNews;
            }

            var news = await GetFeed(_newsFeed);
            if (news != null)
            {
                await _cache.SetAsync(_newsCacheKey, news, _newsCacheExpiration);
            }

            return news;
        }

        public async Task<List<FeedItem>> GetUpdateFeed()
        {
            var cacheUpdates = await _cache.GetAsync<List<FeedItem>>(_updatesCacheKey);
            if (cacheUpdates != null)
            {
                return cacheUpdates;
            }

            var updates = await GetFeed(_updateFeed);
            if (updates != null)
            {
                await _cache.SetAsync(_updatesCacheKey, updates, _updatesCacheExpiration);
            }

            return updates;
        }

        private static async Task<List<FeedItem>> GetFeed(string feedAddress)
        {
            var feedResult = new List<FeedItem>();

            using (var xmlReader = XmlReader.Create(feedAddress, new XmlReaderSettings() { Async = true }))
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
