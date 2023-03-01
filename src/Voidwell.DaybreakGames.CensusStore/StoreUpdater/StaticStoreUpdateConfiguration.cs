using System;
using System.Collections.Generic;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater
{
    public class StaticStoreUpdateConfiguration
    {
        public bool Enabled { get; set; } = true;
        public Type CollectionType { get; set; }
        public Type EntityType { get; set; }
        public string StoreName { get; set; }
        public TimeSpan Period { get; set; } = TimeSpan.FromDays(7);
        public List<Type> Dependencies { get; set; } = new List<Type>();
    }
}
