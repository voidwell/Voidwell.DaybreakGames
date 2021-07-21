using System.Collections.Generic;
using System.Threading.Tasks;

namespace Voidwell.DaybreakGames.CensusStore.StoreUpdater
{
    public interface IStoreUpdaterService
    {
        public IEnumerable<LastStoreUpdate> GetStoreUpdateLog();
        public Task<LastStoreUpdate> UpdateStoreAsync(string storeName);
    }
}