using System.Collections.Generic;
using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services
{
    public interface IFeedService
    {
        Task<List<FeedItem>> GetNewsFeed();
        Task<List<FeedItem>> GetUpdateFeed();
    }
}
