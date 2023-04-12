using System.Threading.Tasks;
using Voidwell.DaybreakGames.Data.Models.Planetside;

namespace Voidwell.DaybreakGames.CensusStore.Services.Abstractions
{
    public interface IMetagameEventStore
    {
        Task<MetagameEventCategory> GetMetagameEventCategoryAsync(int metagameEventId);
        Task<int?> GetMetagameCategoryZoneIdAsync(int metagameEventId);
    }
}