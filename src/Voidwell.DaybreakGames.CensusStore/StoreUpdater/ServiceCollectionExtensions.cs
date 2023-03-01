using Microsoft.Extensions.DependencyInjection;
using Voidwell.DaybreakGames.Census.Collection.Abstract;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater
{
    internal static class ServiceCollectionExtensions
    {
        public static IStaticStoreUpdateBuilder<TCollection> AddUpdaterStaticStore<TCollection, TEntity>(this IServiceCollection services, bool enabled = true, int periodDays = 7)
            where TCollection : class, ICensusStaticCollection
            where TEntity : class
        {
            return new StaticStoreUpdateBuilder<TCollection, TEntity>(services, enabled, periodDays);
        }
    }
}
