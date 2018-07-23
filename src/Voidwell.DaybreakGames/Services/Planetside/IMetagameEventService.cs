using System.Threading.Tasks;
using Voidwell.DaybreakGames.Models;

namespace Voidwell.DaybreakGames.Services.Planetside
{
    public interface IMetagameEventService : IUpdateable
    {
        Task<ZoneMetagameEvent> GetMetagameEvent(int metagameEventId);
    }
}
