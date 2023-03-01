using Voidwell.DaybreakGames.Census.Collection.Abstract;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater
{
    public interface IStaticStoreUpdateBuilder<TCollection>
        where TCollection : class, ICensusStaticCollection
    {
        IStaticStoreUpdateBuilder<TCollection> WithUpdateDependency<TDependency>() where TDependency : class, ICensusStaticCollection;
    }
}