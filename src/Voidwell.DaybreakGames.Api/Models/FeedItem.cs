using System;

namespace Voidwell.DaybreakGames.Api.Models
{
    public class FeedItem
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Link { get; set; }
        public string Author { get; set; }
    }
}
