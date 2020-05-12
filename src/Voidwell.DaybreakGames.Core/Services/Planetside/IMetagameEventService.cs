using System.Threading.Tasks;
using Voidwell.DaybreakGames.Core.Models;

namespace Voidwell.DaybreakGames.Core.Services.Planetside
{
    public interface IMetagameEventService
    {
        Task RefreshStore();
        Task<ZoneMetagameEvent> GetMetagameEvent(int metagameEventId);
    }
}
