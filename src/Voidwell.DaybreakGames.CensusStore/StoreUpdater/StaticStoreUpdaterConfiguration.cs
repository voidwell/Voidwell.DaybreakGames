using System.Collections.Generic;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater
{
    public class StaticStoreUpdaterConfiguration
    {
        public bool Enabled { get; set; } = true;
        public List<StaticStoreUpdateConfiguration> Collections { get; } = new();
    }
}