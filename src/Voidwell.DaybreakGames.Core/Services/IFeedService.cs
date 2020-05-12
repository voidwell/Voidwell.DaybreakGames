using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.Core.Services
{
    public interface IFeedService
    {
        Task<IEnumerable<FeedItem>> GetNewsFeed();
        Task<IEnumerable<FeedItem>> GetUpdateFeed();
    }
}
