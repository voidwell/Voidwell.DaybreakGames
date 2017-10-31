using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services
{
    public class FeedService : IFeedService
    {
        private const string _newsFeed = "https://forums.daybreakgames.com/ps2/index.php?forums/official-news-and-announcements.19/index.rss";
        private const string _updateFeed = "https://forums.daybreakgames.com/ps2/index.php?forums/game-update-notes.73/index.rss";

        public Task<List<FeedItem>> GetNewsFeed()
        {
            return GetFeed(_newsFeed);
        }

        public Task<List<FeedItem>> GetUpdateFeed()
        {
            return GetFeed(_updateFeed);
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
