namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater;

/// <summary>
/// Replaces the default upsert for a store whose entities need custom handling when the store updates.
/// </summary>
public interface IStoreUpdateHandler<TEntity>
    where TEntity : class
{
    Task UpdateAsync(IEnumerable<TEntity> entities);
}
