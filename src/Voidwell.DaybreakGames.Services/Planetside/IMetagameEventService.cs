using System.Threading.Tasks;
using Voidwell.DaybreakGames.Domain.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IMetagameEventService
    {
        Task<ZoneMetagameEvent> GetMetagameEvent(int metagameEventId);
    }
}
