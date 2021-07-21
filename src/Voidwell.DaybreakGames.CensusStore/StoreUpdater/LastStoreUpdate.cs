using System;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater
{
    public class LastStoreUpdate
    {
        public string StoreName { get; set; }
        public DateTime? LastUpdated { get; set; }
        public TimeSpan UpdateInterval { get; set; }
    }
}
