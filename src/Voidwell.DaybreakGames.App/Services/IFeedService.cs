using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services
{
    public interface IFeedService
    {
        Task<IEnumerable<FeedItem>> GetNewsFeed();
        Task<IEnumerable<FeedItem>> GetUpdateFeed();
    }
}
